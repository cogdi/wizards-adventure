using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbarianMediumDistanceState : BarbarianBaseState
{
    public static event Action OnStateChanged;
    public static event Action OnEarthquakeTriggered;

    private float earthquakeTimer;
    private float earthquakeTimerMax = 4f;

    //private bool IsEarthquakeHappening;

    public override void EnterState()
    {
        // earthquakeTimer = earthquakeTimerMax;
    }
    
    public override void PerformState()
    {
        if (stateMachine.Barbarian.GetDistanceToPlayer() <= Barbarian.CLOSE_DISTANCE ||
        stateMachine.Barbarian.GetDistanceToPlayer() > Barbarian.MEDIUM_DISTANCE)
        {
            // The distance got out of the boundaries of medium-distance attacks.
            OnStateChanged?.Invoke();
        }

        earthquakeTimer += Time.deltaTime;
        if (earthquakeTimer >= earthquakeTimerMax)
        {
           Earthquake();
        }

        else if (!stateMachine.Barbarian.IsEarthquakeHappening)
        {
            stateMachine.Agent.SetDestination(PlayerCombat.Instance.transform.position);
        }
    }
    
    private void Earthquake()
    {
        stateMachine.Barbarian.Agent.ResetPath();

        OnEarthquakeTriggered?.Invoke();
        earthquakeTimer = 0f;            
    }



    public override void ExitState()
    {

    }
}
