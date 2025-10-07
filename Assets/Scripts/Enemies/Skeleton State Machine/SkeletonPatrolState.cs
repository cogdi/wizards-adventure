using System.Collections.Generic;
using UnityEngine;

public class SkeletonPatrolState : SkeletonBaseState
{
    private UnityEngine.AI.NavMeshAgent agent;
    private List<Transform> patrolPointList;
    private int currentPatrolPoint;
    private float patrolStandTimer;
    private float patrolStandTimerMax = 2f;

    public override void EnterState()
    {
        agent = skeleton.Agent;

        patrolPointList = skeleton.GetPatrolPointList();
        
        if (patrolPointList != null)
        {

            if (currentPatrolPoint < patrolPointList.Count)
            {
                agent.SetDestination(patrolPointList[currentPatrolPoint].position);
            }

            else
            {
                agent.SetDestination(patrolPointList[0].position);
            }
        }

        SoundManager.Instance.OnAnySoundMade += PlayerCombat_OnWallHit;
    }

    public override void PerformState()
    {
        if (skeleton.CanSeePlayer())
        {
            stateMachine.SwitchState(stateMachine.attackState);
        }

        else if (patrolPointList != null)
        {
            PatrolCycle();
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
        if (Vector3.Distance(skeleton.transform.position, hitPosition) <= 13f)
        {
            stateMachine.SwitchState(stateMachine.searchState);
        }
    }
}