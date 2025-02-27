using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    /* 
     * This class is created to be used by all the types of enemies in the game. 
     * It contains all the mutual logic between enemy classes.
    */

    // Health.
    public float Health => health;
    protected const float MAX_HEALTH = 100f;
    protected float health = MAX_HEALTH;

    // Info about the main character.
    protected Transform playerTransform;
    protected Vector3 playerLastPosition;
    protected PlayerCombat playerCombatInstance;
    protected int ignoreRaycastMask; // It is for the character's shield, clothes, etc.

    protected abstract void TakeDamage(EnemyBase enemy, float damage);
    public abstract bool IsMoving();

    protected virtual void Start()
    {
        playerCombatInstance = PlayerCombat.Instance;
        playerTransform = playerCombatInstance.transform;
        if (playerTransform == null)
        {
            Debug.LogError("Player instance can not be found.");
        }

        PlayerCombat.Instance.OnEnemyDamaged += TakeDamage;
        MagicCharge.OnEnemyDamaged += TakeDamage;

        ignoreRaycastMask = ~LayerMask.GetMask("IgnoreSkeletonRaycast"); // Rename this.
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

