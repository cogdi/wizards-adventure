using System.Resources;
using UnityEngine;
using UnityEngine.AI;

public class Barbarian : EnemyBase
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private BarbarianStateMachine stateMachine;

    private float closeDistance = 2f;
    private float mediumDistance = 10f;
    private float farDistance = 20f;

    public override bool IsMoving()
    {
        return agent.velocity.magnitude > 0.1f;
    }

    // protected override void TakeDamage(EnemyBase enemy, float damage)
    // {
    //     if (enemy == this)
    //     {
    //         health -= damage;
    //         Debug.Log(health);

    //         if (health <= 0f)
    //         {
    //             Destroy(gameObject);
    //         }
    //     }
    // }
    
    
    // public bool CanSeePlayer() // This doesn't need to be here. Remove it as debug is finished.
    // {
    // float eyeLevel = 1.15f;
    // float sightDistance = 15f;
    // float fieldOfView = 100f;
    //     if (GetDistanceToPlayer() <= sightDistance)
    //     {
    //         Vector3 playerDirection = playerTransform.position - transform.position;
    //         if (Vector3.Angle(playerDirection, transform.forward) <= fieldOfView)
    //         {
    //             if (Physics.Raycast(transform.position + (Vector3.up * eyeLevel), playerDirection, out RaycastHit hitInfo, sightDistance, ignoreRaycastMask))
    //             {
    //                 if (playerCombatInstance.IsPlayerLayer(hitInfo.transform.gameObject.layer))
    //                 {
    //                     playerLastPosition = playerTransform.position;
    //                     return true;
    //                 }
    //             }
    //         }
    //     }

    //     return false;
    // }

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
        CheckDistanceToPlayer();
    }

    private void CheckDistanceToPlayer()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= closeDistance)
        {
            Debug.Log("The player is on close distance");
        }

        else if (distance <= mediumDistance)
        {
            Debug.Log("The player is on MeDiUm distance");
        }

        else if (distance <= farDistance)
        {
            Debug.Log("The player is on FAAAAAAAR distance");
        }
    }
}
