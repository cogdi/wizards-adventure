using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbarianMediumDistanceState : BarbarianBaseState
{
    public static event Action OnEarthquakeTriggered;

    private float earthquakeTimer;
    private float earthquakeTimerMax = 4f;

    public override void EnterState()
    {
        earthquakeTimer = earthquakeTimerMax;
    }
    
    public override void PerformState()
    {
        earthquakeTimer += Time.deltaTime;

        if (earthquakeTimer >= earthquakeTimerMax)
        {
            OnEarthquakeTriggered?.Invoke();
            earthquakeTimer = 0f;
            
            stateMachine.Agent.SetDestination(PlayerCombat.Instance.transform.position);
        }
    }
    
    public override void ExitState()
    {

    }
}
