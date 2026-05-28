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
    public event Action OnEarthquakeFinishedEvent;

    

    [SerializeField] private NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; set 
    {
        if (value)
            agent = value; // Debug.
    }}

    public bool IsEarthquakeHappening { get; private set; }

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
    public const float MEDIUM_DISTANCE = 15f;
    public const float EARTHQUAKE_DISTANCE = 10f; // 8f - default.
    [SerializeField] private float meleeDamageDistance = 2.75f;

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
    [SerializeField] private float rushSpeed = 5f;
    [SerializeField] private float regularSpeed = 1.75f; // 1.75f - default value.
    private bool isRushing;
    private bool isDestinationSet;
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
        ApplyRegularSpeed();
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialise();
        BarbarianLongDistanceState.OnRockThrowned += ShootProjectile;
    }

    // private void Update()
    // {
        //Debug.Log("Distance to player: " + GetDistanceToPlayer());
    // }

    public void MeleeDamageToPlayer()
    {
        if (GetDistanceToPlayer() <= meleeDamageDistance)
        {
            OnPlayerHit?.Invoke(CLOSE_DISTANCE_DAMAGE);
        }
    }
    
    public void Earthquake()
    {
        // Called by an animation event.

        if (GetDistanceToPlayer() <= EARTHQUAKE_DISTANCE)
        {
            OnEarthquakeHitPlayer?.Invoke(STUN_DAMAGE);
        }
    }

    public void OnEarthquakeFinished()
    {
        OnEarthquakeFinishedEvent?.Invoke();
    }

    public void ShootProjectile()
    {
        Vector3 direction = playerBody.position - rockSpawnPoint.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 directionToPlayer = (playerBody.position - transform.position).normalized;

        GameObject projectile = Instantiate(Resources.Load($"Prefabs/{BARBARIAN_ROCK}") as GameObject, rockSpawnPoint.position, rotation, parent: this.transform);
        projectile.GetComponent<Rigidbody>().velocity = directionToPlayer * rockThrowingSpeed;
    }

    public bool IsFloorLayer(int layer)
    {
        return floorLayer == (floorLayer | 1 << layer);
    }

    public bool IsWallLayer(int layer)
    {
        return wallLayer == (wallLayer | 1 << layer);
    }

    public void ApplyRushSpeed()
    {
        agent.speed = rushSpeed;
    }

    public void ApplyRegularSpeed()
    {
        agent.speed = regularSpeed;
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
