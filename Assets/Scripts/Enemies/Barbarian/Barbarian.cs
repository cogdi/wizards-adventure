using System;
using System.Resources;
using UnityEngine;
using UnityEngine.AI;

public class Barbarian : EnemyBase
{
    public static event Action<float> OnPlayerHit;
    public event Action OnCloseAttack;
    public event Action OnEarthquakeTriggered;
    
    public event Action OnEarthquakeHitPlayer;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private BarbarianStateMachine stateMachine;

    // Damage.
    private const float CLOSE_DISTANCE_DAMAGE = 15f;

    // Distances.
    //private float closeAttackDistance = 3f;
    private float closeDistance = 5.5f;
    private float earthquakeDistance = 8f;
    private float mediumDistance = 12f;
    //private float farDistance = 12f;

    // Timings.
    private float closeAttackTimer;
    private float closeAttackTimerMax = 3f;

    public override bool IsMoving()
    {
        return agent.velocity.magnitude > 0.1f;
    }

    // protected override void TakeDamage(EnemyBase enemy, float damage)
    // {
    //     if (enemy == this)
    //     {
    //         health -= damage;
    //         Debug.Log(health);

    //         if (health <= 0f)
    //         {
    //             Destroy(gameObject);
    //         }
    //     }
    // }
    
    
    // public bool CanSeePlayer() // This doesn't need to be here. Remove it as debug is finished.
    // {
    // float eyeLevel = 1.15f;
    // float sightDistance = 15f;
    // float fieldOfView = 100f;
    //     if (GetDistanceToPlayer() <= sightDistance)
    //     {
    //         Vector3 playerDirection = playerTransform.position - transform.position;
    //         if (Vector3.Angle(playerDirection, transform.forward) <= fieldOfView)
    //         {
    //             if (Physics.Raycast(transform.position + (Vector3.up * eyeLevel), playerDirection, out RaycastHit hitInfo, sightDistance, ignoreRaycastMask))
    //             {
    //                 if (playerCombatInstance.IsPlayerLayer(hitInfo.transform.gameObject.layer))
    //                 {
    //                     playerLastPosition = playerTransform.position;
    //                     return true;
    //                 }
    //             }
    //         }
    //     }

    //     return false;
    // }

    private void Awake()
    {
        MAX_HEALTH = 300f;
        
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        LookTowards(playerTransform.position);

        float distance = GetDistanceToPlayer();

        if (distance <= closeDistance)
        {
            Debug.Log("Close distance");
            CloseDistanceAttack();
        }

        else if (distance <= mediumDistance)
        {
            Debug.Log("Medium distance");
            MediumDistanceAttack();
        }

        else
        {
            Debug.Log("The player is on FAAAAAAAR distance");
        }
    }

    public override void DamageToPlayer()
    {
        if (GetDistanceToPlayer() <= 2f)
        {
            OnPlayerHit?.Invoke(CLOSE_DISTANCE_DAMAGE);
        }
    }

    private void CloseDistanceAttack()
    {
        closeAttackTimer += Time.deltaTime;

        agent.SetDestination(playerTransform.position);
        
        if (agent.remainingDistance <= agent.stoppingDistance && closeAttackTimer >= closeAttackTimerMax)
        {
            OnCloseAttack?.Invoke();
            closeAttackTimer = 0f;
        }

    }

    private void MediumDistanceAttack()
    {
        OnEarthquakeTriggered?.Invoke();
    }

    private void Earthquake()
    {
        if (GetDistanceToPlayer() <= earthquakeDistance)
        {
            OnEarthquakeHitPlayer?.Invoke();
        }
    }
}
