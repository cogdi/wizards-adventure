using System;
using UnityEngine;

public class RegularDoor : InteractiveDoor
{
    public event Action<bool> OnDoorStateChanged;

    private bool isOpening;
    private bool isClosedByKey = true;
    private bool hasGotKey; // TODO: Make proper logic for doors' keys.

    [SerializeField] private int requiredKeyID;

    protected override void Start()
    {
        Debug.Log("Start()");
        base.Start();

        PlayerMotor.Instance.OnPickingKeys += PickKey;
    }

    private void Update()
    {
        // Debug.
        if (Input.GetKeyDown(KeyCode.K))
        {
            PickKey(requiredKeyID);
        }
    }

    private void PickKey(int keyID)
    {
        Debug.Log(keyID);

        if (keyID == requiredKeyID)
        {
            hasGotKey = true;
        }
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
