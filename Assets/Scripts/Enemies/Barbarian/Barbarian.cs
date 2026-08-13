using System;
using System.Resources;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Threading;

public class Barbarian : EnemyBase
{
    public static event Action<float> OnPlayerHit;
    public static event Action<float> OnEarthquakeHitPlayer;
    public static event Action OnBarbarianBeated;
    
    public event Action OnPlayerHitInRush;
    public event Action OnWallHitInRush;
    
    public event Action OnCloseAttack;
    public event Action OnEarthquakeTriggered;
    public event Action OnEarthquakeFinishedEvent;

    public event Action OnRockCrushed;
    public event Action OnStonesThrowned;

    [SerializeField] private NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; set 
    {
        if (value)
            agent = value; // Debug.
    }}

    public Transform StonesSpawnPoint { get => rockSpawnPoint; }

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
    public List<GameObject> Rocks { get => rocks; }
    public bool AllRocksCrushed { get => rocks.Count <= 0; }
    private const string BARBARIAN_ROCK = "Barbarian_Rock_1";
    private const float STONE_THROWING_SPEED = 40f;
    private const int STONES_COUNT = 50;
    // [SerializeField] private List<GameObject> rocks;
    private List<GameObject> rocks;

    private BarbarianObjectPool<BarbarianStoneProjectile> stonesPool;
    [SerializeField] private GameObject stonePiecePrefab;

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

        rocks = new List<GameObject>();
    }

    private void OnEnable()
    {
        InitializeStonesPool();
    }

    protected override void Start()
    {
        base.Start();

        RockManager.Instance.OnRockFallen += AddRock;

        stateMachine.Initialise();
    }


    protected override void TakeDamage(EnemyBase enemy, float damage)
    {
        if (enemy == this && !BarbarianLongDistanceState.IsRushing)
        {
            health -= damage;
            Debug.Log(health);

            if (health <= 0f)
            {
                OnBeated();
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
    
    private void InitializeStonesPool()
    {
        if (stonesPool == null || stonesPool.Count <= 0)
        {
            stonesPool = new BarbarianObjectPool<BarbarianStoneProjectile>(stonePiecePrefab.GetComponent<BarbarianStoneProjectile>(), STONES_COUNT);

            for (int i = 0; i < STONES_COUNT; i++)
            {
                stonesPool.Get().SetPoolReference(stonesPool);
            }
        }
    }

    public void ThrowStones()
    {
        for (int i = 0; i < STONES_COUNT; i++)
        {
            float yaw = UnityEngine.Random.Range(-15f, 15f);
            float pitch = UnityEngine.Random.Range(-3.5f, 3.5f);

            LookTowards(playerBody.transform.position);

            Vector3 dir = Quaternion.Euler(pitch, yaw, 0) * rockSpawnPoint.forward;

            BarbarianStoneProjectile sp = stonesPool.Get();

            sp.transform.position = StonesSpawnPoint.position;
            sp.transform.rotation = Quaternion.identity;

            // sp.GetComponent<Rigidbody>().velocity = dir * STONE_THROWING_SPEED;
            sp.ShootProjectile(dir, STONE_THROWING_SPEED);
        }

        OnStonesThrowned?.Invoke();
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
        RockManager.Instance.TriggerRockFalling();

        OnEarthquakeFinishedEvent?.Invoke();
    }

    public void PickUpStone()
    {
        if (rocks.Count < 0)
            return;

        rocks[rocks.Count - 1].transform.SetParent(transform);
        rocks[rocks.Count - 1].transform.localPosition = StonesSpawnPoint.position;
    }

    public void ShootOnStoneCrushedEvent()
    {
        // Destroy(stateMachine.Barbarian.rocks[rocks.Count - 1]);
        // stateMachine.Barbarian.rocks.RemoveAt(rocks.Count - 1);
        
        GameObject rock = rocks[rocks.Count - 1];
        stateMachine.Barbarian.rocks.RemoveAt(rocks.Count - 1);
        
        Destroy(rock);

        OnRockCrushed?.Invoke();
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

    public void AddRock(GameObject rock)
    {
        rocks.Add(rock);
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

    private void OnBeated()
    {
        OnBarbarianBeated?.Invoke();

        Destroy(gameObject);
    }
}
