using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BarbarianCloseDistanceState : BarbarianBaseState
{
    public static event Action OnStateChanged;
    public static event Action OnCloseAttack;

    private float closeAttackTimer;
    private float closeAttackTimerMax = 3f;

    private NavMeshAgent agent;
    private Transform playerTransform;

    public override void EnterState()
    {
        //closeAttackTimer = closeAttackTimerMax;
        agent = stateMachine.Agent;
        playerTransform = PlayerCombat.Instance.transform;
    }
    
    public override void PerformState()
    {
        if (stateMachine.Barbarian.GetDistanceToPlayer() > Barbarian.CLOSE_DISTANCE)
        {
            // The distance got out of the boundaries of close attacks.
            OnStateChanged?.Invoke();
        }

        closeAttackTimer += Time.deltaTime;

        agent.SetDestination(playerTransform.position);
        
        if (agent.remainingDistance <= stateMachine.Agent.stoppingDistance &&
        closeAttackTimer >= closeAttackTimerMax)
        {
            OnCloseAttack?.Invoke();
            closeAttackTimer = 0f;
        }
    }

    public override void ExitState()
    {

    }
}
