using UnityEngine;

public class TransitionableDoor : InteractiveDoor
{
    protected override void Start()
    {
        base.Start();
    }

    public override void PlayerMotor_OnDoorInteracted(Transform door)
    {
        if (door == transform)
        {
            Loader.Instance.PerformSceneTransition();
        }
    }
}
