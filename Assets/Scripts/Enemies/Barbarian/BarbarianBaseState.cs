using UnityEngine;

public abstract class BarbarianBaseState : MonoBehaviour
{

    public abstract void EnterState();

    public abstract void PerformState();

    public virtual void ExitState()
    {

    }
}
