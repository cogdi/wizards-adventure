using System;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public enum DodgeTypes
    {
        Left,
        Right,
        Forward,
        Backward   
    }
    
    public static PlayerMotor Instance { get; private set; }

    public event Action<Transform> OnDoorInteracted;
    public event Action<int> OnPickingKeys;
    public event Action<bool> OnThirdPersonModeStateChanged;
    
    public event Action<DodgeTypes> OnPlayerDodged;
    

    private PlayerInput playerInputInstance;
    private PlayerLook playerLookInstance;

    // Movement.
    [SerializeField] private CharacterController controller;
    private float jumpHeight = 2f;
    private float gravity = 9.8f;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isMoving;
    private bool isStandingOnTopOfEnemy;
    private float verticalVelocity = 0f;
    private bool thirdPersonMode = false;
    [SerializeField] private float rotationSpeed = 10f;

    // Flying.
    public bool IsFlying { get => isFlying; }
    private bool isFlying; // Experiment.
    [SerializeField] private float flyingVelocity = 4.5f;

    // Running.
    private float currentSpeed;
    private float walkingSpeed = 3f;
    private float runningSpeed = 6f;
    private bool isRunning;

    // Interactables.
    private int doorLayerMask;
    private int keyLayerMask;
    
    // Dodging.
    private Vector3 lastMoveDirection;
    private float dodgeSpeed = 10f;
    private float dodgeDuration = 0.2f;
    private bool isDodging;
    private float dodgeTimer;
    private Vector3 dodgeDirection;
    private float dodgeCooldown;
    private float dodgeCooldownMax = 2.5f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        currentSpeed = walkingSpeed;
    }

    private void Start()
    {
        doorLayerMask = LayerMask.GetMask("Door");
        keyLayerMask = LayerMask.GetMask("Key");

        playerInputInstance = PlayerInput.Instance;
        playerLookInstance = PlayerLook.Instance;
        playerInputInstance.OnInteractPerformed += Interact;
        playerInputInstance.OnJumpPerformed += Jump;
        playerInputInstance.OnDodgePerformed += Dodge;
    }

    private void Update()
    {
        // Debug.
        if (Input.GetKeyDown(KeyCode.T))
        {
            thirdPersonMode = !thirdPersonMode;

            OnThirdPersonModeStateChanged?.Invoke(thirdPersonMode);
        }

        if (isStandingOnTopOfEnemy)
        {
            PushAwayFromEnemy();
        }
        
        else
        {
            if (thirdPersonMode)
            {
                MoveThirdPerson();
                
                HandleDodging();
            }
            else Move();

            HandleRunning();
        }

        HandleFlying(); // Experiment.
        
        dodgeCooldown += Time.deltaTime;
    }

    private void Move()
    {
        Vector2 inputVector = playerInputInstance.GetMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        if (!isDodging)
        {
            lastMoveDirection = transform.TransformDirection(moveDirection);
            controller.Move(transform.TransformDirection(moveDirection) * (currentSpeed * Time.deltaTime));
        }
        
        velocity.y -= gravity * Time.deltaTime;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        if (!isDodging)
            controller.Move(velocity * Time.deltaTime);

        isGrounded = controller.isGrounded;
    }

    private void MoveThirdPerson()
    {
        Vector2 inputVector = playerInputInstance.GetMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputVector.y, 0f, -inputVector.x); // 3rd-person movement from above.

        if (!isDodging)
        {
            lastMoveDirection = moveDirection;
            controller.Move(moveDirection * (currentSpeed * Time.deltaTime));
        }

        velocity.y -= gravity * Time.deltaTime;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        if (!isDodging)
        {
            controller.Move(velocity * Time.deltaTime);
        }

        isGrounded = controller.isGrounded;
    }

    private void Jump()
    {
        if (isGrounded)
            velocity.y += Mathf.Sqrt(2 * gravity * jumpHeight);
    }
    
    private void Dodge()
    {
        if (!isDodging && dodgeCooldown >= dodgeCooldownMax &&
        !CharacterAttributes.Instance.IsStunned)
        {
            //StartDodging();
            
            if (lastMoveDirection.sqrMagnitude < 0.01f)
            {
                return;
            }

            isDodging = true;
            dodgeTimer = dodgeDuration;
            dodgeDirection = lastMoveDirection;
            
            dodgeCooldown = 0f;
        }
    }

    // private void StartDodging()
    // {
    //     if (lastMoveDirection.sqrMagnitude < 0.01f)
    //     {
    //         return;
    //     }
    //
    //     isDodging = true;
    //     dodgeTimer = dodgeDuration;
    //     dodgeDirection = lastMoveDirection;
    // }
    
    private void HandleDodging()
    {
        if (!isDodging) return;

        float forwardDot = Vector3.Dot(transform.forward, lastMoveDirection);
        float rightDot = Vector3.Dot(transform.right, lastMoveDirection);
        
        if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
        {
            switch (forwardDot)
            {
                case > 0:
                    OnPlayerDodged?.Invoke(DodgeTypes.Forward);
                    break;
                default:
                    OnPlayerDodged?.Invoke(DodgeTypes.Backward);
                    break;
            }
        }
        else
        {
            switch (rightDot)
            {
                case > 0:
                    OnPlayerDodged?.Invoke(DodgeTypes.Right);
                    break;
                default:
                    OnPlayerDodged?.Invoke(DodgeTypes.Left);
                    break;
            }
        }
        
        dodgeTimer -= Time.deltaTime;
        
        controller.Move(dodgeDirection * (dodgeSpeed * Time.deltaTime));

        if (dodgeTimer <= 0f)
        {
            isDodging = false;
        }
    }

    private void HandleFlying()
    {
        isFlying = playerInputInstance.IsFlyingPressed();
        if (isFlying)
        {
            velocity.y = flyingVelocity;
        }
    }

    private void HandleRunning()
    {
        isMoving = playerInputInstance.GetMovementVectorNormalized() != Vector2.zero;

        if (isMoving && playerInputInstance.IsRunningTriggered() && !playerInputInstance.IsBlockingPressed())
        {
            // TODO: Try to make it better.
            if (CharacterAttributes.Instance.IsCharacterAbleToRun())
            {
                currentSpeed = runningSpeed;
                isRunning = true;
            }

            else
            {
                currentSpeed = walkingSpeed;
                isRunning = false;
            }
        }

        else
        {
            currentSpeed = walkingSpeed;
            isRunning = false;
        }
    }

    private void PushAwayFromEnemy()
    {
        Vector3 moveDirection = (Vector3.down - transform.forward).normalized;
        controller.Move(moveDirection * (10f * Time.deltaTime));
    }

    private void Interact()
    {
        Ray ray = new Ray(PlayerLook.Instance.GetCameraPosition(), PlayerLook.Instance.GetCameraTransformForward());

        /* TODO: Make system of distinguishing different doors and objects.
         * It can be reached through using interfaces, like IInteractable, for special potions, keys, doors. */

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 5f, doorLayerMask))
        {
            OnDoorInteracted?.Invoke(hitInfo.transform);
        }

        else if (Physics.Raycast(ray, out RaycastHit hitInfo3, 5f, keyLayerMask))
        {
            int keyID = hitInfo3.transform.GetComponent<Key>().keyID;
            OnPickingKeys?.Invoke(keyID);
            
            Destroy(hitInfo3.transform.gameObject);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isGrounded && PlayerCombat.Instance.IsEnemyLayer(hit.gameObject.layer))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].CompareTag("Floor")) 
                    return;
                // Debug.Log("Player is on top of the Skeleton.");
                isStandingOnTopOfEnemy = true;
            }
        }

        else
        {
            isStandingOnTopOfEnemy = false;
        }
    }

    public bool IsThirdPersonModeActive()
    {
        return thirdPersonMode;
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public void ApplyStunnedSpeed()
    {
        currentSpeed = walkingSpeed / 2;
    }

    public void ApplyNormalSpeed()
    {
        currentSpeed = walkingSpeed;
    }
}
