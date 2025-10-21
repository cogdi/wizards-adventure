public abstract class BarbarianBaseState
{
    public BarbarianStateMachine stateMachine;

    public abstract void EnterState();
    
    public abstract void PerformState();
    
    public virtual void ExitState()
    {

    }
}