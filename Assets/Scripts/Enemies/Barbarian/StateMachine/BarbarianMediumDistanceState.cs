using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbarianMediumDistanceState : BarbarianBaseState
{
    public static event Action OnStateChanged;
    public static event Action OnEarthquakeTriggered;

    private float earthquakeTimer;
    private float earthquakeTimerMax = 8f;

    private bool IsEarthquakeHappening;

    public override void EnterState()
    {
        stateMachine.Barbarian.OnEarthquakeFinishedEvent += Barbarian_OnEarthquakeFinishedEvent;
        //earthquakeTimer = earthquakeTimerMax;
    }

    private void Barbarian_OnEarthquakeFinishedEvent()
    {
        IsEarthquakeHappening = false;
    }

    public override void PerformState()
    {
        if (stateMachine.Barbarian.GetDistanceToPlayer() <= Barbarian.CLOSE_DISTANCE ||
        stateMachine.Barbarian.GetDistanceToPlayer() > Barbarian.MEDIUM_DISTANCE)
        {
            // The distance got out of the boundaries of medium-distance attacks.
            OnStateChanged?.Invoke();
        }

        if (!IsEarthquakeHappening)
        {
            stateMachine.Agent.SetDestination(PlayerCombat.Instance.transform.position);

            earthquakeTimer += Time.deltaTime;
            if (earthquakeTimer >= earthquakeTimerMax)
            {
                Earthquake();
            }
        }
    }
    


    private void Earthquake()
    {
        IsEarthquakeHappening = true;

        stateMachine.Barbarian.Agent.ResetPath();
        OnEarthquakeTriggered?.Invoke();
        
        earthquakeTimer = 0f;            
    }



    public override void ExitState()
    {
        stateMachine.Barbarian.OnEarthquakeFinishedEvent -= Barbarian_OnEarthquakeFinishedEvent;
    }
}
