using UnityEngine;
using UnityEngine.AI;

public class SkeletonSearchState : SkeletonBaseState
{
    private NavMeshAgent agent;

    public float searchPlayerTimer;
    private float searchPlayerTimerMax = 8f;
    private float moveTimer;
    private float moveTimerMax = 2f;

    public override void EnterState()
    {
        agent = skeleton.Agent;

        if (agent != null)
        {
            agent.SetDestination(skeleton.GetPlayerTransform().position);
        }
    }

    public override void PerformState()
    {
        if (!skeleton.CanSeePlayer())
        {
            if (searchPlayerTimer >= searchPlayerTimerMax)
            {
                stateMachine.SwitchState(stateMachine.guardState);
            }

            searchPlayerTimer += Time.deltaTime;
            moveTimer += Time.deltaTime;

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (moveTimer >= moveTimerMax)
                {
                    moveTimer = 0f;
                    agent.SetDestination(skeleton.transform.position + Random.insideUnitSphere * 5f);
                }
            }
        }

        else
        {
            stateMachine.SwitchState(stateMachine.attackState);
        }
    }

    public override void ExitState()
    {
        agent.ResetPath();
        searchPlayerTimer = 0f;
    }
}