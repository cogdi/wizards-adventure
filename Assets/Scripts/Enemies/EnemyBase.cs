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
    protected float angularSpeed = 120f;

    // Info about the main character.
    protected Transform playerTransform;
    protected Transform playerBody;
    protected Vector3 playerLastPosition;
    protected PlayerCombat playerCombatInstance;
    protected int ignoreRaycastMask; // It is for the character's shield, clothes, etc.


    protected abstract void TakeDamage(EnemyBase enemy, float damage);
    public abstract bool IsMoving();

    protected virtual void Start()
    {
        playerCombatInstance = PlayerCombat.Instance;
        playerTransform = playerCombatInstance.transform;
        playerBody = playerCombatInstance.PlayerBody;

        if (playerTransform == null)
        {
            Debug.LogError("Player instance can not be found.");
        }

        PlayerCombat.Instance.OnEnemyDamaged += TakeDamage;
        MagicCharge.OnEnemyDamaged += TakeDamage;

        ignoreRaycastMask = ~LayerMask.GetMask("IgnoreSkeletonRaycast"); // Rename this.
    }
    private void OnDestroy()
    {
        PlayerCombat.Instance.OnEnemyDamaged -= TakeDamage;
        MagicCharge.OnEnemyDamaged -= TakeDamage;
    }

    protected void LookTowards(Vector3 point)
    {
        Vector3 lookDirection = GetNormalizedDirectionTo(point);
        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, angularSpeed * Time.deltaTime);
        }
    }

    protected Vector3 GetNormalizedDirectionTo(Vector3 point)
    {
        return (point - transform.position).normalized;
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public float GetDistanceToPlayer()
    {
        return Vector3.Distance(playerTransform.position, transform.position);
    }

}

