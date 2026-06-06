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
    /* Combined MEDIUM_DISTANCE AND EARTHQUAKE_DISTANCE at one const.
       It's not meaningful to keep them as separate properties,
       because the Barbarian will stand at one place when got out of EARTHQUAKE_DISTANCE boundaries. */

    [SerializeField] private float meleeDamageDistance = 2.75f;

    // Throwing rocks.
    private const string BARBARIAN_ROCK = "Barbarian_Rock";
    private const float rockThrowingSpeed = 40f;
    private bool isRockThrowed;

    // Rush.
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float rushSpeed = 5f;
    [SerializeField] private float regularSpeed = 1.75f; // 1.75f - default value.

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

    protected override void TakeDamage(EnemyBase enemy, float damage)
    {
        if (enemy == this && !BarbarianLongDistanceState.IsRushing)
        {
            health -= damage;
            Debug.Log(health);

            if (health <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

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

        if (GetDistanceToPlayer() <= MEDIUM_DISTANCE)
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

    private bool IsMagicCharge(GameObject go)
    {
        return  go.CompareTag(PlayerCombat.LIGHT_MAGIC_CHARGE_TAG) ||
                go.CompareTag(PlayerCombat.MEDIUM_MAGIC_CHARGE_TAG) ||
                go.CompareTag(PlayerCombat.STRONG_MAGIC_CHARGE_TAG);
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
                if (!IsMagicCharge(collision.collider.gameObject))
                {
                    OnWallHitInRush?.Invoke();
                }
            }
        }
    }
}
