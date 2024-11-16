using System;
using UnityEngine;

public class Barbarian : Enemy
{
    public static Barbarian Instance { get; private set; }
    private BarbarianAnimations barbarianAnimationsInstance;
    public static event Action<float> OnDamagingPlayer;

    private float patrolTime = 0f;
    private float meleeDamage = 15f;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
        }
    }

    protected override void Start()
    {
        base.Start();

        barbarianAnimationsInstance = BarbarianAnimations.Instance;
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            AttackPlayer();
        }

        else Patrol();
    }

    private void Patrol()
    {
        patrolTime += Time.deltaTime;

        if (patrolTime >= 5f)
        {
            agent.SetDestination(transform.position + UnityEngine.Random.insideUnitSphere * 5f);
            patrolTime = 0f;
        }
    }

    private void AttackPlayer()
    {
        //agent.autoRepath // TODO: Check out what is this and other fields and methods of NavMeshAgent.

        if (!barbarianAnimationsInstance.IsAttackingAnimationPlaying)
        {
            if (GetDistanceToPlayer() >= 4f)
            {
                agent.SetDestination(playerTransform.position);
            }

            else
            {
                agent.ResetPath();
                barbarianAnimationsInstance.TriggerAttackingPlayer();
            }
        }
    }

    public void DamagePlayer()
    {
        OnDamagingPlayer?.Invoke(meleeDamage);
    }
}