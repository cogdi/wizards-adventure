using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BarbarianStateMachine : MonoBehaviour
{
    public BarbarianCloseDistanceState BarbarianCloseDistanceState;
    public BarbarianMediumDistanceState BarbarianMediumDistanceState;
    public BarbarianLongDistanceState BarbarianLongDistanceState;

    BarbarianBaseState? currentState;

    public Barbarian Barbarian { get; private set; }
    public NavMeshAgent Agent { get; set; }
    public float DistanceToPlayer { get; private set; }

    public void Initialise()
    {
        BarbarianCloseDistanceState = new BarbarianCloseDistanceState();
        BarbarianMediumDistanceState = new BarbarianMediumDistanceState();
        BarbarianLongDistanceState = new BarbarianLongDistanceState();

        BarbarianCloseDistanceState.OnStateChanged += OnStateChanged;
        BarbarianMediumDistanceState.OnStateChanged += OnStateChanged;
        BarbarianLongDistanceState.OnStateChanged += OnStateChanged;


        Barbarian = GetComponent<Barbarian>();
        Agent = Barbarian.Agent;

        DistanceToPlayer = Barbarian.GetDistanceToPlayer();

        SwitchState(BarbarianCloseDistanceState);
    }

    private void Update()
    {
        // if (Barbarian.GetDistanceToPlayer() <= Barbarian.CLOSE_DISTANCE)
        // {
        //     SwitchState(BarbarianCloseDistanceState);
        // }

        // else if (Barbarian.GetDistanceToPlayer() <= Barbarian.MEDIUM_DISTANCE)
        // {
        //     SwitchState(BarbarianMediumDistanceState);
        // }

        // else
        // {
        //     SwitchState(BarbarianLongDistanceState);
        // }

        //DistanceToPlayer = barbarian.GetDistanceToPlayer();
        
        //Debug.Log(currentState);
        currentState.PerformState();
    }

    private void SwitchState(BarbarianBaseState state)
    {
        if (state != null)
        {
            currentState?.ExitState();

            currentState = state;
            currentState.stateMachine = this;
            currentState.EnterState();
        }
    }

    private void OnStateChanged()
    {
        if (!BarbarianLongDistanceState.IsRushing)
        {
            if (Barbarian.GetDistanceToPlayer() <= Barbarian.CLOSE_DISTANCE)
            {
                SwitchState(BarbarianCloseDistanceState);
            }

            else if (Barbarian.GetDistanceToPlayer() <= Barbarian.MEDIUM_DISTANCE)
            {
                SwitchState(BarbarianMediumDistanceState);
            }

            else
            {
                SwitchState(BarbarianLongDistanceState);
            }
        }
    }

    private void OnDestroy()
    {
        currentState.ExitState();
    }
}
