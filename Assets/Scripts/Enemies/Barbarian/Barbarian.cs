using System;
using System.Resources;
using UnityEngine;
using UnityEngine.AI;

public class Barbarian : EnemyBase
{
    public static event Action<float> OnPlayerHit;
    public static event Action<float> OnEarthquakeHitPlayer;

    public event Action OnCloseAttack;
    public event Action OnEarthquakeTriggered;
    

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private BarbarianStateMachine stateMachine;

    // Weapons.
    [SerializeField] private GameObject leftHandWeapon;
    [SerializeField] private GameObject rightHandWeapon;
    [SerializeField] private GameObject twoHandedWeapon;
    [SerializeField] private Transform rockSpawnPoint;

    // Damage.
    public const float STONES_DAMAGE = 30f;
    private const float CLOSE_DISTANCE_DAMAGE = 15f;
    private const float STUN_DAMAGE = 5f;

    // Distances.
    //private float closeAttackDistance = 3f;
    private float closeDistance = 5.5f;
    private float earthquakeDistance = 8f;
    private float mediumDistance = 12f;
    //private float farDistance = 12f;

    // Timings.
    private float closeAttackTimer;
    private float closeAttackTimerMax = 3f;
    private float earthquakeTimer;
    private float earthquakeTimerMax = 4f;

    // Throwing rocks.
    private const string BARBARIAN_ROCK = "Barbarian_Rock";
    private bool isRockThrowed;

    // Rush.
    [SerializeField] private LayerMask floorLayer;
    private bool isRushing;
    private float regularSpeed = 1.75f;
    private float rushSpeed = 5f;
    private float rushTimer;
    private float rushTimerMax = 7f;

    public override bool IsMoving()
    {
        return agent.velocity.magnitude > 0.1f;
    }

    private void Awake()
    {
        MAX_HEALTH = 300f;
        agent.speed = regularSpeed;
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
            Debug.Log("Far distance");
            LongDistanceAttack();
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
        earthquakeTimer += Time.deltaTime;

        if (earthquakeTimer >= earthquakeTimerMax)
        {
            OnEarthquakeTriggered?.Invoke();
            earthquakeTimer = 0f;
            
            agent.SetDestination(playerTransform.position);
        }
    }

    private void LongDistanceAttack()
    {
        if (!isRockThrowed)
        {
            ShootProjectile();
            isRockThrowed = true;
        }

        else
        {
            Rush();
        }
    }

    public void Earthquake()
    {
        if (GetDistanceToPlayer() <= earthquakeDistance)
        {
            OnEarthquakeHitPlayer?.Invoke(STUN_DAMAGE);
        }
    }

    public void ShootProjectile()
    {
        Vector3 direction = playerBody.position - rockSpawnPoint.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 directionToPlayer = (playerBody.position - transform.position).normalized;

        GameObject projectile;

        /* |3f| - Mistake in throwing the stone. 
        40f - speed of the projectile. */
        
        projectile = Instantiate(Resources.Load($"Prefabs/{BARBARIAN_ROCK}") as GameObject, rockSpawnPoint.position, rotation, parent: this.transform);
        projectile.GetComponent<Rigidbody>().velocity = directionToPlayer * 40f;
    }

    public void Rush()
    {
        rushTimer += Time.deltaTime;

        if (rushTimer >= rushTimerMax)
        {
            agent.speed = rushSpeed;

            //Vector3 directionToPlayer = (playerBody.position - transform.position).normalized;
            LookTowards(playerTransform.position);
            agent.SetDestination(playerTransform.position);
            isRushing = true;
            // //int numHitColliders = Physics.OverlapSphereNonAlloc(attackRangeSphere.position, attackRangeSphereRadius, hitColliders);
            // int numHitColliders = Physics.OverlapSphereNonAlloc(transform.position, 0.5f, hitColliders);
            // for (int i = 0; i < numHitColliders; i++)
            // {
            //     Collider collider = hitColliders[i];
            //     if (playerCombatInstance.IsPlayerLayer(collider.gameObject.layer))
            //     {
            //         // collider.TryGetComponent<EnemyBase>(out EnemyBase enemy);
            //         // if (enemy != null)
            //         // {
            //         //     OnEnemyHit?.Invoke(collider.ClosestPointOnBounds(attackRangeSphere.position));
            //         //     OnEnemyDamaged?.Invoke(enemy, meleeDamage);
            //         // }

            //         Debug.Log("Hit player");
            //         agent.speed = 0.1f;
            //     }

            //     else if (!IsFloorLayer())
            //     {
            //         // if (collider != null)
            //         // {
            //         //     OnWallHit?.Invoke(collider.ClosestPointOnBounds(attackRangeSphere.position));
            //         // }
            //         Debug.Log("Hit wall");
            //         agent.speed = 0.1f;
            //     }
            // }
        }
    }

    public bool IsFloorLayer(int layer)
    {
        return floorLayer == (floorLayer | 1 << layer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isRushing)
        {
            if (playerCombatInstance.IsPlayerLayer(collision.collider.gameObject.layer))
            {
                Debug.Log("Hit player");
                agent.speed = 0.1f;

                isRushing = false;
                rushTimer = 0f;

            }

            else if (!IsFloorLayer(collision.gameObject.layer))
            {
                // if (collider != null)
                // {
                //     OnWallHit?.Invoke(collider.ClosestPointOnBounds(attackRangeSphere.position));
                // }
                Debug.Log("Hit wall");
                agent.speed = 0.1f;

                isRushing = false;
                rushTimer = 0f;
            }
        }
    }
}
