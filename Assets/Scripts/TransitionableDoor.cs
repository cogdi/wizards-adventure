using UnityEngine;

public class TransitionableDoor : Door
{
    protected override void Start()
    {
        base.Start();
    }

    public override void PlayerMotor_OnDoorInteracted(Transform door)
    {
        if (door == transform)
        {
            Debug.Log("Changing scene...");
            Loader.Instance.PerformSceneTransition();
        }
    }
}
