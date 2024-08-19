using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class SkeletonStateMachine : MonoBehaviour
{
    public SkeletonPatrolState patrolState;
    public SkeletonAttackState attackState;
    public SkeletonSearchState searchState;
    public SkeletonGuardState guardState;

    private Skeleton skeleton;

    private SkeletonBaseState currentState;
    //private bool isSwitchingStates;
    //private SkeletonBaseState nextState;

    public void Initialise()
    {
        // Initialized from SkeletonPatrolState.
        skeleton = GetComponent<Skeleton>();
        patrolState = new SkeletonPatrolState();
        attackState = new SkeletonAttackState();
        searchState = new SkeletonSearchState();
        guardState = new SkeletonGuardState();

        if (skeleton.CompareTag(Skeleton.MELEE_SKELETON_TAG))
            SwitchState(patrolState);
        else SwitchState(guardState);
    }

    private void Update()
    {
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
}
