using UnityEngine;
using UnityEngine.AI;

public class SkeletonStateMachine : MonoBehaviour
{
    public SkeletonPatrolState patrolState;

    public SkeletonGuardState guardState;
    public SkeletonAttackState attackState;
    public SkeletonSearchState searchState;

    [SerializeField] private Skeleton skeleton;
    private SkeletonBaseState currentState;
    //private bool isSwitchingStates;
    //private SkeletonBaseState nextState;

    public void Initialise()
    {
        // Initialized from Skeleton.cs.
        patrolState = new SkeletonPatrolState();

        guardState = new SkeletonGuardState();
        attackState = new SkeletonAttackState();
        searchState = new SkeletonSearchState();

        if (skeleton.IsMeleeSkeleton)
            SwitchState(patrolState);
        else SwitchState(guardState);

        // if (skeleton.IsMeleeSkeleton) SwitchState()
        // else SwitchState(guardState);
    }

    private void Update()
    {
        //Debug.Log(skeleton.gameObject.ToString() + currentState);
        currentState?.PerformState();
    }

    public void SwitchState(SkeletonBaseState newState)
    {
        if (newState != null)
        {
            currentState?.ExitState();

            currentState = newState;
            currentState.stateMachine = this;
            currentState.skeleton = skeleton;
            currentState.EnterState();
        }
    }

    public SkeletonBaseState GetCurrentState()
    {
        return currentState;
    }

    public void OnDestroy()
    {
        currentState.ExitState();
    }
}
