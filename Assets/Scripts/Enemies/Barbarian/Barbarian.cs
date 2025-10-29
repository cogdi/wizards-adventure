using System;
using System.Resources;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Barbarian : EnemyBase
{
    public static event Action<float> OnPlayerHit;
    public static event Action<float> OnEarthquakeHitPlayer;
    
    public event Action OnPlayerHitInRush;
    public event Action OnWallHitInRush;



    public event Action OnCloseAttack;
    public event Action OnEarthquakeTriggered;
    

    [SerializeField] private NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; set 
    {
        agent = value; // Debug.
    }}

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
    public const float CLOSE_DISTANCE = 5.5f;
    public const float MEDIUM_DISTANCE = 12f;
    public const float earthquakeDistance = 8f;

    // Timings.
    private float closeAttackTimer;
    private float closeAttackTimerMax = 3f;
    private float earthquakeTimer;
    private float earthquakeTimerMax = 4f;

    // Throwing rocks.
    private const string BARBARIAN_ROCK = "Barbarian_Rock";
    private const float rockThrowingSpeed = 40f;
    private bool isRockThrowed;

    // Rush.
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask wallLayer;
    private bool isRushing;
    private bool isDestinationSet;
    private float regularSpeed = 1.75f;
    private float rushSpeed = 5f;
    private const float rushTimerMax = 4f;
    private float rushTimer = rushTimerMax;
    private bool lookingAtPlayerDirection;

    // State Machine.
    [SerializeField] private BarbarianStateMachine stateMachine;

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

        stateMachine.Initialise();
        BarbarianLongDistanceState.OnRockThrowned += ShootProjectile;
    }

    // private void Update()
    // {
    //     float distance = GetDistanceToPlayer();

    //     if (!isRushing)
    //     {
    //         if (distance <= CLOSE_DISTANCE)
    //         {
    //             CloseDistanceAttack();
    //         }

    //         else if (distance <= MEDIUM_DISTANCE)
    //         {
    //             MediumDistanceAttack();
    //         }

    //         else
    //         {
    //             Debug.Log("Long-dist 1111");
    //             LongDistanceAttack();
    //         }
    //     }

    //     else
    //     {
    //         Debug.Log("Long-dist 2222");
    //         LongDistanceAttack();
    //     }

    //     Debug.Log(isRushing);
    // }

    public void MeleeDamageToPlayer()
    {
        if (GetDistanceToPlayer() <= 2.25f)
        {
            OnPlayerHit?.Invoke(CLOSE_DISTANCE_DAMAGE);
        }
    }

    // private void CloseDistanceAttack()
    // {
    //     closeAttackTimer += Time.deltaTime;

    //     agent.SetDestination(playerTransform.position);
        
    //     if (agent.remainingDistance <= agent.stoppingDistance && closeAttackTimer >= closeAttackTimerMax)
    //     {
    //         OnCloseAttack?.Invoke();
    //         closeAttackTimer = 0f;
    //     }

    // }

    // private void MediumDistanceAttack()
    // {
    //     earthquakeTimer += Time.deltaTime;

    //     if (earthquakeTimer >= earthquakeTimerMax)
    //     {
    //         OnEarthquakeTriggered?.Invoke();
    //         earthquakeTimer = 0f;
            
    //         agent.SetDestination(playerTransform.position);
    //     }
    // }

    // private void LongDistanceAttack()
    // {
    //     if (!isRockThrowed)
    //     {
    //         ShootProjectile();
    //         isRockThrowed = true;
    //     }

    //     else
    //     {
    //         StartCoroutine(Rush());
    //     }
    // }

    public void Earthquake()
    {
        // Called by an animation event.

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

        GameObject projectile = Instantiate(Resources.Load($"Prefabs/{BARBARIAN_ROCK}") as GameObject, rockSpawnPoint.position, rotation, parent: this.transform);
        projectile.GetComponent<Rigidbody>().velocity = directionToPlayer * rockThrowingSpeed;
    }

    // public IEnumerator Rush()
    // {
    //     if (!lookingAtPlayerDirection)
    //     {
    //         LookTowards(playerTransform.position);
            
    //         yield return new WaitForSeconds(2);

    //         lookingAtPlayerDirection = true;
    //     }

    //     rushTimer += Time.deltaTime;

    //     if (rushTimer >= rushTimerMax)
    //     {
    //         agent.speed = rushSpeed;
    //         isRushing = true;

    //         Vector3 targetPoint = transform.position + transform.forward * 5f;
    //         if (NavMesh.SamplePosition(targetPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
    //         {
    //             agent.SetDestination(hit.position);
    //         }
    //         else
    //         {
    //             Debug.Log("Navmesh ended");

    //             StopRushing();
    //         }
    //     }
    // }

    // public void StopRushing()
    // {
    //     agent.ResetPath();
    //     agent.speed = regularSpeed;
    //     isRushing = false;
    //     lookingAtPlayerDirection = false;
    //     rushTimer = 0f;
    // }

    public bool IsFloorLayer(int layer)
    {
        return floorLayer == (floorLayer | 1 << layer);
    }

    public bool IsWallLayer(int layer)
    {
        return wallLayer == (wallLayer | 1 << layer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (BarbarianLongDistanceState.IsRushing)
        {
            if (playerCombatInstance.IsPlayerLayer(collision.collider.gameObject.layer))
            {
                OnPlayerHitInRush?.Invoke();
            }

            else if (!IsFloorLayer(collision.collider.gameObject.layer))
            {
                OnWallHitInRush?.Invoke();
            }
        }
    }
}
