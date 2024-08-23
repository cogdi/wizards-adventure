using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

//public class Skeleton : MonoBehaviour
//{
//    // Skeleton class will be used for the next types of enemies, with almost exact same logic (except patroling and various attacks):
//    // SkeletonMinion, SkeletonWarrior, SkeletonArcher, SkeletonMage.

//    public event Action OnAttackingPlayer;
//    public static event Action<float> OnPlayerHit;

//    public const string MELEE_SKELETON_TAG = "MeleeSkeleton";
//    public const string RANGED_SKELETON_TAG = "RangedSkeleton";

//    public const string SKELETON_ARCHER_TAG = "SkeletonArcher";
//    public const string SKELETON_MAGE_TAG = "SkeletonMage";

//    public const float ARROW_DAMAGE = 16f;
//    public const float MAGIC_DAMAGE = 20f;
//    public const float MELEE_DAMAGE = 10f;

//    public float Health => health;
//    public SkeletonStateMachine StateMachine { get => stateMachine; }
//    public NavMeshAgent Agent { get => agent; }

//    //private const string SKELETON_ARCHER_NAME = "SkeletonArcher";
//    //private const string SKELETON_MAGE_NAME = "SkeletonMage";
//    private const string ARROW = "Arrow";
//    private const string SKELETON_MAGIC_CHARGE = "SkeletonMagicCharge";
//    private const float MAX_HEALTH = 100f;
//    private const float arrowSpeed = 40f;
//    private const float magicChargeSpeed = 10f;

//    [SerializeField] private List<Transform> patrolPointsList;
//    [SerializeField] private Transform projectileSpawnPoint;
//    [SerializeField] private Transform playerBody;
//    [SerializeField] private Transform guardPoint;
//    private float health = 0f;
//    private SkeletonStateMachine stateMachine;
//    private NavMeshAgent agent;
//    private Transform playerTransform;
//    private Vector3 playerLastPosition;

//    // Raycast
//    private float eyeLevel = 1.15f;
//    private float sightDistance = 15f;
//    private float fieldOfView = 100f;
//    private int ignoreRaycastMask;

//    private void Awake()
//    {
//        health = MAX_HEALTH;
//        agent = GetComponent<NavMeshAgent>();
//        agent.stoppingDistance = 0.1f;
//    }

//    private void Start()
//    {
//        playerTransform = GameObject.FindGameObjectWithTag(PlayerCombat.PLAYER_TAG).transform;
//        if (playerTransform == null)
//        {
//            Debug.LogError("Player instance can not be found.");
//        }

//        stateMachine = GetComponent<SkeletonStateMachine>();
//        stateMachine.Initialise();

//        ignoreRaycastMask = ~LayerMask.GetMask("IgnoreSkeletonRaycast");

//        PlayerCombat.Instance.OnSkeletonDamaged += TakeDamage;
//        MagicCharge.OnSkeletonDamaged += MagicCharge_OnSkeletonDamaged;
//    }

//    public void InvokeOnAttackingPlayerEvent()
//    {
//        OnAttackingPlayer?.Invoke();
//    }

//    public bool CanSeePlayer()
//    {
//        if (GetDistanceToPlayer() <= sightDistance)
//        {
//            Vector3 playerDirection = playerTransform.position - transform.position;
//            if (Vector3.Angle(playerDirection, transform.forward) <= fieldOfView)
//            {
//                if (Physics.Raycast(transform.position + (Vector3.up * eyeLevel), playerDirection, out RaycastHit hitInfo, sightDistance, ignoreRaycastMask))
//                {
//                    //Debug.Log(hitInfo.transform);
//                    if (hitInfo.transform.CompareTag(PlayerCombat.PLAYER_TAG))//(hitInfo.transform == playerTransform)
//                    {
//                        playerLastPosition = playerTransform.position;
//                        return true;
//                    }
//                }
//            }
//        }

//        return false;
//    }

//    public Vector3 GetPlayerLastPosition()
//    {
//        return playerLastPosition;
//    }

//    public bool IsMoving()
//    {
//        return agent.velocity.magnitude > 0.1f;
//    }

//    public List<Transform> GetPatrolPointList()
//    {
//        return patrolPointsList;
//    }

//    public Transform GetGuardPoint()
//    {
//        return guardPoint;
//    }

//    public Transform GetPlayerTransform()
//    {
//        return playerTransform;
//    }

//    // Called by the animation event on ranged skeletons.
//    // This methos isn't in the SkeletonAttackState.cs because AnimationEvent can't access it (it must be SkeletonMinion component).
//    public void ShootProjectile()
//    {
//        Vector3 direction = playerBody.position - projectileSpawnPoint.position;
//        Quaternion rotation = Quaternion.LookRotation(direction);
//        Vector3 directionToPlayer = (playerBody.position - transform.position).normalized;

//        GameObject projectile;

//        if (CompareTag(SKELETON_ARCHER_TAG))
//        {
//            /* 3f - Mistakes in archer's aim.
//             * 40f - Force added to velocity. */
//            projectile = Instantiate(Resources.Load($"Prefabs/{ARROW}") as GameObject, projectileSpawnPoint.position, rotation);
//            projectile.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(UnityEngine.Random.Range(-3f, 3f), Vector3.up) * directionToPlayer * arrowSpeed;
//        }

//        else if (CompareTag(SKELETON_MAGE_TAG))
//        {
//            projectile = Instantiate(Resources.Load($"Prefabs/{SKELETON_MAGIC_CHARGE}") as GameObject, projectileSpawnPoint.position, rotation);
//            projectile.GetComponent<Rigidbody>().velocity = directionToPlayer * magicChargeSpeed;
//        }
//    }

//    // Called by the animation event.
//    public void DamageToPlayer()
//    {
//        if (GetDistanceToPlayer() <= 2f)
//        {
//            OnPlayerHit?.Invoke(MELEE_DAMAGE);
//        }
//    }

//    private void MagicCharge_OnSkeletonDamaged(object sender, MagicCharge.OnSkeletonDamagedEventArgs e)
//    {
//        TakeDamage(e.skeleton, e.damage);
//    }

//    private void TakeDamage(Skeleton skeleton, float damage)
//    {
//        if (skeleton == this)
//        {
//            // To get the Skeleton know about the player's position.
//            // TODO: Make a proper system with audio. If any sound plays in some range (10f), skeletons will hear it and come to it.

//            health -= damage;
//            Debug.Log(skeleton.health);

//            if (health <= 0f)
//            {
//                Destroy(gameObject);
//            }
//        }
//    }

//    private void OnDestroy()
//    {
//        MagicCharge.OnSkeletonDamaged -= MagicCharge_OnSkeletonDamaged;
//    }

//    public float GetDistanceToPlayer()
//    {
//        return Vector3.Distance(playerTransform.position, transform.position);
//    }
//}






public class Skeleton : Enemy
{
    // Skeleton class will be used for the next types of enemies, with almost exact same logic (except patroling and various attacks):
    // SkeletonMinion, SkeletonWarrior, SkeletonArcher, SkeletonMage.

    public event Action OnAttackingPlayer;
    public static event Action<float> OnPlayerHit;

    public const string MELEE_SKELETON_TAG = "MeleeSkeleton";
    //public const string RANGED_SKELETON_TAG = "RangedSkeleton";

    public const string SKELETON_ARCHER_TAG = "SkeletonArcher";
    public const string SKELETON_MAGE_TAG = "SkeletonMage";

    public const float ARROW_DAMAGE = 16f;
    public const float MAGIC_DAMAGE = 20f;
    public const float MELEE_DAMAGE = 10f;

    public SkeletonStateMachine StateMachine { get => stateMachine; }

    protected const string ARROW = "Arrow";
    protected const string SKELETON_MAGIC_CHARGE = "SkeletonMagicCharge";
    protected const float arrowSpeed = 40f;
    protected const float magicChargeSpeed = 10f;

    [SerializeField] protected List<Transform> patrolPointsList;
    [SerializeField] protected Transform projectileSpawnPoint;
    [SerializeField] protected Transform playerBody;
    //[SerializeField] private Transform guardPoint;
    protected SkeletonStateMachine stateMachine;

    protected override void Start()
    {
        base.Start();

        stateMachine = GetComponent<SkeletonStateMachine>();
        stateMachine.Initialise();

        PlayerCombat.Instance.OnSkeletonDamaged += TakeDamage;
        MagicCharge.OnSkeletonDamaged += MagicCharge_OnSkeletonDamaged;
    }

    public void InvokeOnAttackingPlayerEvent()
    {
        OnAttackingPlayer?.Invoke();
    }

    public List<Transform> GetPatrolPointList()
    {
        return patrolPointsList;
    }

    // Called by the animation event on ranged skeletons.
    // This methos isn't in the SkeletonAttackState.cs because AnimationEvent can't access it (it must be SkeletonMinion component).
    public void ShootProjectile()
    {
        Vector3 direction = playerBody.position - projectileSpawnPoint.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 directionToPlayer = (playerBody.position - transform.position).normalized;

        GameObject projectile;

        if (CompareTag(SKELETON_ARCHER_TAG))
        {
            /* 3f - Mistakes in archer's aim.
             * 40f - Force added to velocity. */
            projectile = Instantiate(Resources.Load($"Prefabs/{ARROW}") as GameObject, projectileSpawnPoint.position, rotation);
            projectile.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(UnityEngine.Random.Range(-3f, 3f), Vector3.up) * directionToPlayer * arrowSpeed;
        }

        else if (CompareTag(SKELETON_MAGE_TAG))
        {
            projectile = Instantiate(Resources.Load($"Prefabs/{SKELETON_MAGIC_CHARGE}") as GameObject, projectileSpawnPoint.position, rotation);
            projectile.GetComponent<Rigidbody>().velocity = directionToPlayer * magicChargeSpeed;
        }
    }

    // Called by the animation event on SkeletonMinion.
    public void DamageToPlayer()
    {
        if (GetDistanceToPlayer() <= 2f)
        {
            OnPlayerHit?.Invoke(MELEE_DAMAGE);
        }
    }

    protected void MagicCharge_OnSkeletonDamaged(object sender, MagicCharge.OnSkeletonDamagedEventArgs e)
    {
        TakeDamage(e.skeleton, e.damage);
    }

    protected void OnDestroy()
    {
        MagicCharge.OnSkeletonDamaged -= MagicCharge_OnSkeletonDamaged;
    }
}