using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class SkeletonStateMachine : MonoBehaviour
{
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
        guardState = new SkeletonGuardState();
        attackState = new SkeletonAttackState();
        searchState = new SkeletonSearchState();

        SwitchState(guardState);
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
