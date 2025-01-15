using UnityEngine;

public abstract class Door : MonoBehaviour
{
    protected virtual void Start()
    {
        PlayerMotor.Instance.OnDoorInteracted += PlayerMotor_OnDoorInteracted;
    }

    public abstract void PlayerMotor_OnDoorInteracted(Transform transform);
}