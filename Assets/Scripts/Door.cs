using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    public event Action<bool> OnDoorStateChanged;

    private bool isOpening;
    private bool isClosedByKey = true;
    private bool hasGotKey; // TODO: Make proper logic for doors' keys.

    private void Start()
    {
        PlayerMotor.Instance.OnDoorInteracted += PlayerMotor_OnDoorInteracted;
        PlayerMotor.Instance.OnPickingKeys += PlayerMotor_OnPickingKeys;
    }

    private void PlayerMotor_OnPickingKeys()
    {
        hasGotKey = true;
    }

    private void PlayerMotor_OnDoorInteracted(Transform door)
    {
        // if transitionable door...
        // else
        TryOpenDoor(door);
    }

    private void TryOpenDoor(Transform door)
    {
        if ((!isClosedByKey || hasGotKey) && door == transform)
        {
            isOpening = !isOpening;
            OnDoorStateChanged?.Invoke(isOpening);

            isClosedByKey = false;
            hasGotKey = false;
        }

        else if (isClosedByKey && !hasGotKey)
        {
            Debug.Log("No keys!");
        }
    }
}
