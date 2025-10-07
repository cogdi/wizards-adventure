using UnityEngine;

public abstract class BarbarianBaseState
{
    private BarbarianStateMachine stateMachine;

    public abstract void EnterState();

    public abstract void PerformState();

    public virtual void ExitState()
    {

    }
}
