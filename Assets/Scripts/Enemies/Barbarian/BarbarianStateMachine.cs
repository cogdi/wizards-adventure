using System;
using UnityEngine;

public class BarbarianStateMachine : MonoBehaviour
{
    public event Action<BarbarianBaseState> OnStateChanged;
    public BarbarianFirstPhaseState firstPhase;

    public void Initialize()
    {
        firstPhase = new BarbarianFirstPhaseState();
    }

    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
