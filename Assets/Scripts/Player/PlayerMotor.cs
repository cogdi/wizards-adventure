using System;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public static PlayerMotor Instance { get; private set; }

    public event Action<Transform> OnDoorInteracted;

    private PlayerInput playerInputInstance;

    // Movement.
    private CharacterController controller;
    private float jumpHeight = 1.5f;
    private float gravity = -9.8f;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isMoving;
    private bool isStandingOnTopOfEnemy;

    // Running.
    private float currentSpeed;
    private float walkingSpeed = 3f;
    private float runningSpeed = 6f;

    // Interactables
    private int interactableLayerMask;

    private void Awake()
    {
        Instance = this;

        Cursor.lockState = CursorLockMode.Locked;

        currentSpeed = walkingSpeed;
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        interactableLayerMask = LayerMask.GetMask("Interactable");

        playerInputInstance = PlayerInput.Instance;
        playerInputInstance.OnInteractPerformed += PlayerInput_OnInteractPerformed;
    }

    private void PlayerInput_OnInteractPerformed()
    {
        Interact();
    }

    //float time = 0f;
    private void Update()
    {
        //time += Time.deltaTime;
        //if (time >= 3f)
        //{
        //    time = 0f;
        //}


        if (isStandingOnTopOfEnemy)
        {
            PushAwayFromEnemy();
        }

        else
        {
            isGrounded = controller.isGrounded;

            Move();

            if (isGrounded)
            {
                if (playerInputInstance.IsJumpTriggered())
                {
                    Jump();
                }

                HandleRunning();
            }
        }

        //HandleMagicAttacks();
    }

    private void Move()
    {
        Vector2 inputVector = playerInputInstance.GetMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(-2 * gravity * jumpHeight);
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
            }

            else
            {
                currentSpeed = walkingSpeed;
            }
        }

        else
        {
            currentSpeed = walkingSpeed;
        }
    }

    public bool IsCharacterRunning()
    {
        return currentSpeed == runningSpeed;
    }

    private void PushAwayFromEnemy()
    {
        Vector3 moveDirection = (Vector3.down - transform.forward).normalized;
        controller.Move(moveDirection * 10f * Time.deltaTime);
    }

    private void Interact()
    {
        Debug.Log("InteractionStarted");

        Ray ray = new Ray(PlayerLook.Instance.GetCameraPosition(), PlayerLook.Instance.GetCameraTransformForward());

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 5f, interactableLayerMask))
        {
            OnDoorInteracted?.Invoke(hitInfo.transform);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Проверяем, столкнулись ли с каким-то объектом-противником
        if (isGrounded && PlayerCombat.Instance.IsEnemyLayer(hit.gameObject.layer))
        {
            // Получаем все коллайдеры в радиусе вокруг игрока
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);

            DebugDrawSphere(transform.position, 1f, Color.green);

            // Обрабатываем каждое столкновение в массиве
            foreach (Collider collider in colliders)
            {
                // Делаем что-то с каждым столкновением, например, выводим информацию о столкновении
                if (collider.CompareTag("Floor")) return;
                Debug.Log("Player is on top of the Skeleton.");
                isStandingOnTopOfEnemy = true;
            }

            // colliders = null;
        }

        else
        {
            isStandingOnTopOfEnemy = false;
        }
    }

    private void DebugDrawSphere(Vector3 position, float radius, Color color)
    {
        Debug.DrawLine(position + Vector3.up * radius, position + Vector3.down * radius, color);
        Debug.DrawLine(position + Vector3.left * radius, position + Vector3.right * radius, color);
        Debug.DrawLine(position + Vector3.forward * radius, position + Vector3.back * radius, color);
    }
}
