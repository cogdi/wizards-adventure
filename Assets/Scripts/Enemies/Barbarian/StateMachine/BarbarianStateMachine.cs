using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BarbarianStateMachine : MonoBehaviour
{
    BarbarianCloseDistanceState BarbarianCloseDistanceState;

    BarbarianBaseState? currentState;

    public Barbarian barbarian { get; private set; }
    public NavMeshAgent Agent { get; private set; }


    public void Initialise()
    {
        BarbarianCloseDistanceState = new BarbarianCloseDistanceState();

        barbarian = GetComponent<Barbarian>();
        Agent = barbarian.Agent;

        SwitchState(BarbarianCloseDistanceState);
    }

    private void Update()
    {
        currentState.PerformState();
    }

    private void SwitchState(BarbarianBaseState state)
    {
        if (state == null)
        {
            currentState?.ExitState();

            currentState = state;
            currentState.stateMachine = this;
            currentState.EnterState();
        }
    }

    private void OnDestroy()
    {
        currentState.ExitState();
    }
}
