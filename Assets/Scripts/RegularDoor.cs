using System;
using UnityEngine;

public class RegularDoor : Door
{
    public event Action<bool> OnDoorStateChanged;

    private bool isOpening;
    private bool isClosedByKey = true;
    private bool hasGotKey; // TODO: Make proper logic for doors' keys.

    protected override void Start()
    {
        Debug.Log("Start()");
        base.Start();

        PlayerMotor.Instance.OnPickingKeys += PlayerMotor_OnPickingKeys;
    }

    private void PlayerMotor_OnPickingKeys()
    {
        hasGotKey = true;
    }

    public override void PlayerMotor_OnDoorInteracted(Transform door)
    {
        if (door == transform)
        {
            if ((!isClosedByKey || hasGotKey))
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
}
