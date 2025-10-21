using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbarianCloseDistanceState : BarbarianBaseState
{
    public static event Action OnCloseAttack;

    private float closeAttackTimer;
    private float closeAttackTimerMax = 3f;

    public override void EnterState()
    {
        closeAttackTimer = closeAttackTimerMax;
    }
    
    public override void PerformState()
    {
        closeAttackTimer += Time.deltaTime;

        stateMachine.Agent.SetDestination(PlayerCombat.Instance.transform.position);
        
        if (stateMachine.Agent.remainingDistance <= stateMachine.Agent.stoppingDistance &&
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
