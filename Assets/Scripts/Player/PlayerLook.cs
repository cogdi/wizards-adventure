using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public static PlayerLook Instance { get; private set; }

    private PlayerInput playerInputInstance;
    private PlayerMotor playerMotorInstance;

    [SerializeField] private Camera cam;
    [SerializeField] private Camera thirdPersonCamera;
    private const float CAMERA_UPPER_CLAMP = -90f;
    private const float CAMERA_LOWER_CLAMP = 54f;
    private float sensitivity = 20f;
    private float xRotation;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Start()
    {
        playerInputInstance = PlayerInput.Instance;
        playerMotorInstance = PlayerMotor.Instance;

        playerMotorInstance.OnThirdPersonModeStateChanged += PlayerMotorInstance_OnThirdPersonModeStateChanged;
    }

    private void LateUpdate()
    {
        if (!playerMotorInstance.IsThirdPersonModeActive())
            FirstPersonLook();

        else
        {
            LookAtMousePosition();
        }
    }

    private void LookAtMousePosition()
    {
        if (Physics.Raycast(thirdPersonCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
        {
            Vector3 pos = hitInfo.point;
            pos.y = transform.position.y;
            transform.LookAt(pos);
        }
    }

    private void PlayerMotorInstance_OnThirdPersonModeStateChanged(bool thirdPerson)
    {
        if (thirdPerson)
            ActivateThirdPersonCamera();
        else ActivateFirstPersonCamera();
    }

    private void ActivateThirdPersonCamera()
    {
        cam.gameObject.SetActive(false);
        thirdPersonCamera.gameObject.SetActive(true);
    }

    private void ActivateFirstPersonCamera()
    {
        thirdPersonCamera.gameObject.SetActive(false);
        cam.gameObject.SetActive(true);
    }

    private void FirstPersonLook()
    {
        float mouseX = playerInputInstance.GetLookingAxis().x * sensitivity * Time.deltaTime;
        float mouseY = playerInputInstance.GetLookingAxis().y * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, CAMERA_UPPER_CLAMP, CAMERA_LOWER_CLAMP);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    public Vector3 GetCameraPosition()
    {
        return cam.transform.position;
    }

    public Vector3 GetCameraTransformForward()
    {
        return cam.transform.forward;
    }
}
