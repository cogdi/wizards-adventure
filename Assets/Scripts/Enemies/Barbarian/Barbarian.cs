using System.Resources;
using UnityEngine;
using UnityEngine.AI;

public class Barbarian : EnemyBase
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private BarbarianStateMachine stateMachine;
    private BarbarianBaseState activeState;

    public override bool IsMoving()
    {
        return agent.velocity.magnitude > 0.1f;
    }

    protected override void TakeDamage(EnemyBase enemy, float damage)
    {
        if (enemy == this)
        {
            health -= damage;
            Debug.Log(health);

            if (health <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }


    private void Awake()
    {
        MAX_HEALTH = 300f;

        activeState = new BarbarianFirstPhaseState();
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.OnStateChanged += BarbarianStateMachine_OnStateChanged;
    }

    private void BarbarianStateMachine_OnStateChanged(BarbarianBaseState state)
    {
        activeState.ExitState();
        activeState = state;
        state.EnterState();
    }

    private void Update()
    {
        activeState.PerformState();
    }
}
