using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    //// Skeleton class will be used for the next types of enemies, with almost exact same logic(except patroling and various attacks) :
    //// SkeletonMinion, SkeletonWarrior, SkeletonArcher, SkeletonMage.

    /* 
     * This class is created to be used by all the types of enemies in the game. 
     * It contains all the mutual logic between enemy classes.
    */

    public event Action OnAttackingPlayer;
    public static event Action<float> OnPlayerHit;

    public abstract void AttackPlayer();

    //public const string MELEE_SKELETON_TAG = "MeleeSkeleton";
    //public const string RANGED_SKELETON_TAG = "RangedSkeleton";

    //public const string SKELETON_ARCHER_TAG = "SkeletonArcher";
    //public const string SKELETON_MAGE_TAG = "SkeletonMage";

    //public const float ARROW_DAMAGE = 16f;
    //public const float MAGIC_DAMAGE = 20f;
    //public const float MELEE_DAMAGE = 10f;

    public float Health => health;
    //public SkeletonStateMachine StateMachine { get => stateMachine; }
    public NavMeshAgent Agent { get => agent; }

    ////private const string SKELETON_ARCHER_NAME = "SkeletonArcher";
    ////private const string SKELETON_MAGE_NAME = "SkeletonMage";
    //private const string ARROW = "Arrow";
    //private const string SKELETON_MAGIC_CHARGE = "SkeletonMagicCharge";
    private const float MAX_HEALTH = 100f;
    //private const float arrowSpeed = 40f;
    //private const float magicChargeSpeed = 10f;

    //[SerializeField] private List<Transform> patrolPointsList;
    //[SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Transform playerBody;
    //[SerializeField] private Transform guardPoint;
    private float health = 0f;
    //private SkeletonStateMachine stateMachine;
    private NavMeshAgent agent;
    private Transform playerTransform;
    private Vector3 playerLastPosition;

    // Raycast
    private float eyeLevel = 1.15f;
    private float sightDistance = 15f;
    private float fieldOfView = 100f;
    private int ignoreRaycastMask;

    protected virtual void Awake()
    {
        health = MAX_HEALTH;
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0.1f;
    }

    protected virtual void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag(PlayerCombat.PLAYER_TAG).transform;
        if (playerTransform == null)
        {
            Debug.LogError("Player instance can not be found.");
        }

        ignoreRaycastMask = ~LayerMask.GetMask("IgnoreSkeletonRaycast");

        PlayerCombat.Instance.OnSkeletonDamaged += TakeDamage;
        MagicCharge.OnSkeletonDamaged += MagicCharge_OnSkeletonDamaged;
    }

    protected virtual void MagicCharge_OnSkeletonDamaged(object sender, MagicCharge.OnSkeletonDamagedEventArgs e)
    {
        TakeDamage(e.skeleton, e.damage);
    }

    protected virtual void TakeDamage(Skeleton enemy, float damage)
    {
        throw new NotImplementedException();

        if (enemy == this)
        {
            // To get the Skeleton know about the player's position.
            // TODO: Make a proper system with audio. If any sound plays in some range (10f), skeletons will hear it and come to it.

            health -= damage;
            Debug.Log(health);

            if (health <= 0f)
            {
                Destroy(gameObject);
            }
        }
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
                    //Debug.Log(hitInfo.transform);
                    if (hitInfo.transform.CompareTag(PlayerCombat.PLAYER_TAG))//(hitInfo.transform == playerTransform)
                    {
                        playerLastPosition = playerTransform.position;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public Vector3 GetPlayerLastPosition()
    {
        return playerLastPosition;
    }

    public bool IsMoving()
    {
        return agent.velocity.magnitude > 0.1f;
    }

    //public List<Transform> GetPatrolPointList()
    //{
    //    return patrolPointsList;
    //}

    //public Transform GetGuardPoint()
    //{
    //    return guardPoint;
    //}

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public float GetDistanceToPlayer()
    {
        return Vector3.Distance(playerTransform.position, transform.position);
    }

    private void OnDestroy()
    {
        MagicCharge.OnSkeletonDamaged -= MagicCharge_OnSkeletonDamaged;
    }
}
