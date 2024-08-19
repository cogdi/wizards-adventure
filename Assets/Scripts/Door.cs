using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    public event Action<bool> OnDoorStateChanged;

    private bool isOpening;
    private bool isClosedByKey;
    private bool hasGotKey = true; // TODO: Make proper logic for doors' keys.

    private void Start()
    {
        PlayerMotor.Instance.OnDoorInteracted += PlayerMotor_OnDoorInteracted;
    }

    private void PlayerMotor_OnDoorInteracted(Transform door)
    {
        if ((!isClosedByKey || hasGotKey) && door == transform)
        {
            isOpening = !isOpening;
            OnDoorStateChanged?.Invoke(isOpening);
        }
    }
}
