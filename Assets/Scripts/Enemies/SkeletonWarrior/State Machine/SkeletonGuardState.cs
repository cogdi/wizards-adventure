using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonGuardState : SkeletonBaseState
{
    // This state is used both by melee and ranged skeletons.
    // The difference is that melee's patrol and the other's don't.

    private NavMeshAgent agent;
    private List<Transform> patrolPointList;
    private int currentPatrolPoint;
    private float patrolStandTimer;
    private float patrolStandTimerMax = 2f;

    private bool isAtGuardPoint;

    public override void EnterState()
    {
        agent = skeleton.Agent;
        patrolPointList = skeleton.GetPatrolPointList();

        if (currentPatrolPoint < patrolPointList.Count)
        {
            agent.SetDestination(patrolPointList[currentPatrolPoint].position);
        }

        else
        {
            agent.SetDestination(patrolPointList[0].position);
        }

        SoundManager.Instance.OnAnySoundMade += PlayerCombat_OnWallHit;
    }

    public override void PerformState()
    {
        if (skeleton.CanSeePlayer())
        {
            stateMachine.SwitchState(stateMachine.attackState);
        }

        else
        {
            if (skeleton.IsMeleeSkeleton)
            {
                PatrolCycle();
            }

            else if (!isAtGuardPoint)
            {
                SendAgentToGuardPost();
            }
        }
    }

    private void SendAgentToGuardPost()
    {
        agent.SetDestination(patrolPointList[0].position);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.ResetPath();
            skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, patrolPointList[0].rotation, Time.deltaTime * 10f);

            if (skeleton.transform.rotation == patrolPointList[0].rotation)
                isAtGuardPoint = true;
        }
    }

    private void PatrolCycle()
    {
        if (agent.remainingDistance < 0.2f)
        {
            patrolStandTimer += Time.deltaTime;

            if (patrolStandTimer >= patrolStandTimerMax)
            {
                if (currentPatrolPoint < patrolPointList.Count)
                {
                    agent.SetDestination(patrolPointList[currentPatrolPoint].position);
                    currentPatrolPoint++;
                }

                else
                {
                    currentPatrolPoint = 0;
                }

                patrolStandTimer = 0f;
            }
        }
    }

    private void PlayerCombat_OnWallHit(Vector3 hitPosition)
    {
        if (skeleton != null && stateMachine.GetCurrentState() == stateMachine.guardState) // TODO: Check if this works with less checks.
        {
            if (Vector3.Distance(skeleton.transform.position, hitPosition) <= 13f)
            {
                stateMachine.SwitchState(stateMachine.searchState);
            }
        }
    }

    public override void ExitState()
    {
        isAtGuardPoint = false;
    }
}
