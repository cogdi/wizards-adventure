using System;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{
    public static PlayerMotor Instance { get; private set; }

    public event Action<Transform> OnDoorInteracted;
    public event Action<int> OnPickingKeys;

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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Cursor.lockState = CursorLockMode.Locked;

        currentSpeed = walkingSpeed;
    }

    private void Start()
    {
        doorLayerMask = LayerMask.GetMask("Door");
        keyLayerMask = LayerMask.GetMask("Key");

        playerInputInstance = PlayerInput.Instance;
        playerLookInstance = PlayerLook.Instance;
        playerInputInstance.OnInteractPerformed += PlayerInput_OnInteractPerformed;
    }

    private void PlayerInput_OnInteractPerformed()
    {
        Interact();
    }

    private void Update()
    {
        //isGrounded = controller.isGrounded;

        if (isStandingOnTopOfEnemy)
        {
            PushAwayFromEnemy();
        }
        
        else
        {
            Move();

            if (isGrounded)
            {
                if (playerInputInstance.IsJumpTriggered())
                {
                    Jump();
                }

                //HandleRunning(); // To disable running while jumping.
            }
            HandleRunning();
        }

        HandleFlying(); // Experiment.
        Debug.Log(isGrounded);
    }

    private void Move()
    {
        Vector2 inputVector = playerInputInstance.GetMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);

        velocity.y -= gravity * Time.deltaTime;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        controller.Move(velocity * Time.deltaTime);

        isGrounded = controller.isGrounded;
    }

    private void Jump()
    {
        velocity.y += Mathf.Sqrt(2 * gravity * jumpHeight);
    }

    private void HandleFlying()
    {
        isFlying = playerInputInstance.IsFlyingPressed();
        if (isFlying)
        {
            //currentSpeed = walkingSpeed;

            //velocity.y = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocity.y = flyingVelocity;
        }

        //else velocity.y += gravity * Time.deltaTime;
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

    public bool IsCharacterRunning()
    {
        return isRunning;
    }

    private void PushAwayFromEnemy()
    {
        Vector3 moveDirection = (Vector3.down - transform.forward).normalized;
        controller.Move(moveDirection * 10f * Time.deltaTime);
    }

    private void Interact()
    {
        Ray ray = new Ray(PlayerLook.Instance.GetCameraPosition(), PlayerLook.Instance.GetCameraTransformForward());

        /* TODO: (High Priority) Make system of distinguishing different doors.
         * and different objects, too.
         * It can be reached through using interfaces, like IInteractable, for special potions, keys, doors.
        */

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

            //DebugDrawSphere(transform.position, 1f, Color.green);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Floor")) 
                    return;
                Debug.Log("Player is on top of the Skeleton.");
                isStandingOnTopOfEnemy = true;
            }
        }

        else
        {
            isStandingOnTopOfEnemy = false;
        }
    }
}
