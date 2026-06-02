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
    
    public bool IsGrounded { get => isGrounded; }

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
    // private float verticalVelocity = 0f;
    // [SerializeField] private float rotationSpeed = 10f;

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

    // Third-person movement.
    /* "isPlayerNearWall" checks, if the player is in fact near the walls that let them see behind them.
        It's the walls that go from Armory and PreMainHall rooms. 
        
        "nearWallControls" is needed to give a signal that we should change controls behaviour.
        See ThirdPersonMove() for more context.
        */
    private bool isPlayerNearWall;
    private bool nearWallControls; 
    private bool thirdPersonMode;
    private Vector2 thirdPersonInputVector;

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        currentSpeed = walkingSpeed;

        thirdPersonInputVector = new Vector2();
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

        CameraPerspectiveTrigger.OnPlayerNearWall += CameraPerspectiveTrigger_OnPlayerNearWall;
    }

    private void CameraPerspectiveTrigger_OnPlayerNearWall(bool state)
    {
        isPlayerNearWall = state;
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
                ThirdPersonMove();
                
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
    
    private void ThirdPersonMove()
    {
        // TODO: Refactor this method and get rid of this many if-else's.

        Vector2 inputVector = playerInputInstance.GetMovementVectorNormalized();

        if (inputVector.magnitude < 1f)
        {
            /* It goes here, because we should only change the controls, when the player is not
                using them. In other way, the player goes in an infinite loop that won't
                let him get pass camera transition. */
            nearWallControls = isPlayerNearWall;
        }

        if (nearWallControls)
        {
            thirdPersonInputVector.y = -inputVector.y;
            thirdPersonInputVector.x = inputVector.x;
        }

        else
        {
            thirdPersonInputVector.y = inputVector.y;
            thirdPersonInputVector.x = -inputVector.x;
        }

        Vector3 moveDirection = new Vector3(thirdPersonInputVector.y, 0f, thirdPersonInputVector.x); // 3rd-person movement from above.

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
