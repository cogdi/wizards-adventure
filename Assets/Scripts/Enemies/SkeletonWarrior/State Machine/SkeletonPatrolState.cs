using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonPatrolState : SkeletonBaseState
{
    private NavMeshAgent agent;
    private List<Transform> patrolPointList;
    private int currentPatrolPoint;
    private float patrolStandTimer;
    private float patrolStandTimerMax = 2f;

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

        //PlayerCombat.Instance.OnWallHit += PlayerCombat_OnWallHit;
        SoundManager.Instance.OnAnySoundMade += PlayerCombat_OnWallHit;
    }

    public override void PerformState()
    {
        PatrolCycle();

        if (skeleton.CanSeePlayer())
        {
            stateMachine.SwitchState(stateMachine.attackState);
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
        if (skeleton != null && stateMachine.GetCurrentState() == stateMachine.patrolState)
        {
            if (Vector3.Distance(skeleton.transform.position, hitPosition) <= 13f)
            {
                stateMachine.SwitchState(stateMachine.searchState);
            }
        }
    }
}
