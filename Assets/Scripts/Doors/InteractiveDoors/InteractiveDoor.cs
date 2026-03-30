using UnityEngine;

public abstract class InteractiveDoor : MonoBehaviour
{
    protected virtual void Start()
    {
        PlayerMotor.Instance.OnDoorInteracted += PlayerMotor_OnDoorInteracted;
    }

    public abstract void PlayerMotor_OnDoorInteracted(Transform transform);
}