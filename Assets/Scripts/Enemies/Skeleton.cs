using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton : Enemy
{
    // Skeleton class will be used for the next types of enemies, with almost exact same logic (except patroling and various attacks):
    // SkeletonMinion, SkeletonWarrior, SkeletonArcher, SkeletonMage.

    public event Action OnAttackingPlayer;
    public static event Action<float> OnPlayerHit;

    public const float ARROW_DAMAGE = 16f;
    public const float MAGIC_DAMAGE = 20f;
    public const float MELEE_DAMAGE = 10f;

    public SkeletonStateMachine StateMachine { get => stateMachine; }
    public bool IsMeleeSkeleton { get; private set; }
    public bool IsSkeletonArcher { get; private set; }
    public bool IsSkeletonMage { get; private set; }

    private const string MELEE_SKELETON_TAG = "MeleeSkeleton";
    private const string SKELETON_ARCHER_TAG = "SkeletonArcher";
    private const string SKELETON_MAGE_TAG = "SkeletonMage";
    private const string ARROW = "Arrow";
    private const string SKELETON_MAGIC_CHARGE = "SkeletonMagicCharge";
    private const float arrowSpeed = 40f;
    private const float magicChargeSpeed = 10f;

    [SerializeField] private List<Transform> patrolPointsList;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Transform playerBody;
    [SerializeField] private SkeletonStateMachine stateMachine;

    protected override void Awake()
    {
        base.Awake();

        IsMeleeSkeleton = CompareTag(MELEE_SKELETON_TAG);
        IsSkeletonArcher = CompareTag(SKELETON_ARCHER_TAG);
        IsSkeletonMage = CompareTag(SKELETON_MAGE_TAG);
    }

    protected override void Start()
    {
        base.Start();

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
    // This method is not in the SkeletonAttackState.cs because AnimationEvent can't access it (it must be SkeletonMinion component).
    public void ShootProjectile()
    {
        Vector3 direction = playerBody.position - projectileSpawnPoint.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 directionToPlayer = (playerBody.position - transform.position).normalized;

        GameObject projectile;

        if (IsSkeletonArcher)
        {
            /* 3f - Mistakes in archer's aim.
             * 40f - Force added to velocity. */
            projectile = Instantiate(Resources.Load($"Prefabs/{ARROW}") as GameObject, projectileSpawnPoint.position, rotation);
            projectile.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(UnityEngine.Random.Range(-3f, 3f), Vector3.up) * directionToPlayer * arrowSpeed;
        }

        else if (IsSkeletonMage)
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

    private void MagicCharge_OnSkeletonDamaged(Skeleton skeleton, float damage)
    {
        TakeDamage(skeleton, damage);
    }

    private void OnDestroy()
    {
        MagicCharge.OnSkeletonDamaged -= MagicCharge_OnSkeletonDamaged;
    }
}