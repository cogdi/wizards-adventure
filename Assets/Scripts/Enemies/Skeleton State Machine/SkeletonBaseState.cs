public abstract class SkeletonBaseState
{
    public Skeleton skeleton;

    public SkeletonStateMachine stateMachine;

    public abstract void EnterState();

    public abstract void PerformState();

    public virtual void UnsubscribeFromEvents()
    {

    }

    public virtual void ExitState()
    {
        UnsubscribeFromEvents();
    }
}
