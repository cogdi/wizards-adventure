using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public static PlayerLook Instance { get; private set; }

    [SerializeField] private Camera cam;
    private const float CAMERA_UPPER_CLAMP = -90f;
    private const float CAMERA_LOWER_CLAMP = 54f;
    private float sensitivity = 20f;
    private float xRotation;

    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        Look();
    }

    private void Look()
    {
        float mouseX = PlayerInput.Instance.GetLookingAxis().x * sensitivity * Time.deltaTime;
        float mouseY = PlayerInput.Instance.GetLookingAxis().y * sensitivity * Time.deltaTime;

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
