using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    /* 
     * This class is created to be used by all the types of enemies in the game. 
     * It contains all the mutual logic between enemy classes.
    */

    public float Health => health;
    public NavMeshAgent Agent { get => agent; }
    
    protected const float MAX_HEALTH = 100f;

    [SerializeField] protected NavMeshAgent agent;
    protected float health = 0f;
    protected Transform playerTransform;
    protected Vector3 playerLastPosition;
    protected PlayerCombat playerCombatInstance;

    // Raycast
    protected float eyeLevel = 1.15f;
    protected float sightDistance = 15f;
    protected float fieldOfView = 100f;
    protected int ignoreRaycastMask;

    protected virtual void Awake()
    {
        health = MAX_HEALTH;
        agent.stoppingDistance = 0.1f;
    }

    protected virtual void Start()
    {
        playerCombatInstance = PlayerCombat.Instance;
        playerTransform = playerCombatInstance.transform;
        if (playerTransform == null)
        {
            Debug.LogError("Player instance can not be found.");
        }

        ignoreRaycastMask = ~LayerMask.GetMask("IgnoreSkeletonRaycast");

        PlayerCombat.Instance.OnEnemyDamaged += TakeDamage;
        MagicCharge.OnEnemyDamaged += TakeDamage;
    }

    protected void TakeDamage(Enemy enemy, float damage) // TODO: Maybe make it abstract.
    {
        if (enemy == this)
        {
            // To get the Skeleton know about the player's position.
            // TODO: Make a proper system with audio. If any sound plays in some range (10f), skeletons will hear it and come to it.

            health -= damage;
            Debug.Log(health);

            if (health <= 0f)
            {
                // TODO: Make proper logic and visual.
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
                    if (playerCombatInstance.IsPlayerLayer(hitInfo.transform.gameObject.layer))
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
        PlayerCombat.Instance.OnEnemyDamaged -= TakeDamage;
        MagicCharge.OnEnemyDamaged -= TakeDamage;
    }
}

