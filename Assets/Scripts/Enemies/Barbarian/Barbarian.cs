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
    private bool isRockEquipped;

    public override bool IsMoving()
    {
        return agent.velocity.magnitude > 0.1f;
    }

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
        if (!isRockEquipped)
        {
            GameObject projectile;

            projectile = Instantiate(Resources.Load($"Prefabs/Barbarian_Rock") as GameObject, rockSpawnPoint.position, transform.rotation);
            isRockEquipped = true;
        //projectile.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(UnityEngine.Random.Range(-3f, 3f), Vector3.up) * directionToPlayer * arrowSpeed;   
        }
    }

    public void Earthquake()
    {
        if (GetDistanceToPlayer() <= earthquakeDistance)
        {
            OnEarthquakeHitPlayer?.Invoke(STUN_DAMAGE);
        }
    }
}
