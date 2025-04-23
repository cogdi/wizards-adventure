using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Barbarian : EnemyBase
{
    public static Barbarian Instance { get; private set; }
    public static event Action<float> OnDamagingPlayer;

    private enum Phase
    {
        First,
        Second,
        Third
    }

    private BarbarianAnimations barbarianAnimationsInstance;
    // Moving.
    public NavMeshAgent Agent { get => agent; }
    private float patrolTime = 0f;

    // Raycast.
    private float eyeLevel = 1.15f;
    private float sightDistance = 15f;
    private float fieldOfView = 100f;
    [SerializeField] public NavMeshAgent agent;

    // Attacking.
    private Collider[] hitColliders = new Collider[10];
    [SerializeField] private Transform attackRangeSphere;
    [SerializeField] private float attackRangeSphereRadius = 0.5f;
    private float meleeDamage = 15f;

    // Boss-fight.
    [SerializeField] private Transform[] skeletonsSpawnPoints;
    private int spawnedSkeletonCount;

    [SerializeField] private static Dictionary<Transform, Transform> a = new();

    // Fast-run.
    //private bool fastRun;
    private float fastRunTimer;
    private float fastRunPrepareTime = 5f;
    private float fastRunTimerMax = 15f;

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
        if (fastRunTimer <= fastRunPrepareTime)
        {
            fastRunTimer += Time.deltaTime;
            AttackPlayer();
        }

        else FastRun();
    }

    private void FastRun()
    {
        fastRunTimer += Time.deltaTime;

        if (fastRunTimer < fastRunTimerMax)
        {
            // TODO: Trigger preparing animation here.
            LookTowards(playerBody.position);

            return;
        }   

        agent.speed *= 2;

        // TODO: Trigger fast-run animation here.

        agent.SetDestination(playerTransform.position);

        // TODO: If the Barbarian reached the player set the timer to zero.
    }

    private void OnDrawGizmosSelected()
    {
        if (attackRangeSphere != null)
        {
            Gizmos.DrawWireSphere(attackRangeSphere.position, attackRangeSphereRadius);
        }
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

    private void SpawnSkeleton(Vector3 position)
    {
        Instantiate(Resources.Load($"Prefabs/SkeletonMage") as GameObject, position, Quaternion.LookRotation(Vector3.zero));
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
            if (GetDistanceToPlayer() >= 2.5f)
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
        int numHitColliders = Physics.OverlapSphereNonAlloc(attackRangeSphere.position, attackRangeSphereRadius, hitColliders);

        for (int i = 0; i < numHitColliders; i++)
        {
            Collider collider = hitColliders[i];
            if (PlayerCombat.Instance.IsPlayerLayer(collider.gameObject.layer))
            {
                OnDamagingPlayer?.Invoke(meleeDamage);
            }
        }
    }

    public override bool IsMoving()
    {
        return agent.velocity.magnitude > 0.1f;
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
}