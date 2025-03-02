using System;
using UnityEngine;
using UnityEngine.AI;

public class Barbarian : EnemyBase
{
    public static Barbarian Instance { get; private set; }
    public NavMeshAgent Agent { get => agent; }

    private BarbarianAnimations barbarianAnimationsInstance;
    public static event Action<float> OnDamagingPlayer;

    private float patrolTime = 0f;
    private float meleeDamage = 15f;

    private float eyeLevel = 1.15f;
    private float sightDistance = 15f;
    private float fieldOfView = 100f;
    [SerializeField] public NavMeshAgent agent;

    private void Awake()
    {
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

    public bool CanSeePlayer()
    {
        if (GetDistanceToPlayer() <= sightDistance)
        {
            Vector3 playerDirection = playerTransform.position - transform.position;
            if (Vector3.Angle(playerDirection, transform.forward) <= fieldOfView)
            {
                if (Physics.Raycast(transform.position + (Vector3.up * eyeLevel), playerDirection, out RaycastHit hitInfo, sightDistance, ignoreRaycastMask))
                {
                    if (playerCombatInstance.IsPlayerLayer(hitInfo.transform.gameObject.layer))
                    {
                        playerLastPosition = playerTransform.position;
                        return true;
                    }
                }
            }
        }

        return false;
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

    public override bool IsMoving()
    {
        return agent.velocity.magnitude > 0.1f;
    }
}