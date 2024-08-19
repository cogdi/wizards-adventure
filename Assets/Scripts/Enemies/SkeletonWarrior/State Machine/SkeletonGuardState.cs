using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonGuardState : SkeletonBaseState
{
    private NavMeshAgent agent;
    private Transform guardPoint;
    private bool isAtGuardPoint;

    public override void EnterState()
    {
        agent = skeleton.Agent;

        isAtGuardPoint = false;
        guardPoint = skeleton.GetGuardPoint();
        agent.SetDestination(guardPoint.position);

        //PlayerCombat.Instance.OnWallHit += PlayerCombat_OnWallHit;
        SoundManager.Instance.OnAnySoundMade += PlayerCombat_OnWallHit;
    }

    public override void PerformState()
    {
        if (!isAtGuardPoint)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                isAtGuardPoint = true;

                skeleton.gameObject.transform.rotation = guardPoint.rotation;
            }
        }

        if (skeleton.CanSeePlayer())
        {
            stateMachine.SwitchState(stateMachine.attackState);
        }
    }

    private void PlayerCombat_OnWallHit(Vector3 hitPosition)
    {
        if (skeleton != null && stateMachine.GetCurrentState() == stateMachine.guardState)
        {
            if (Vector3.Distance(skeleton.transform.position, hitPosition) <= 13f)
            {
                stateMachine.SwitchState(stateMachine.searchState);
            }
        }
    }
}