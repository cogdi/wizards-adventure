using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Witch : EnemyBase
{
    public event Action OnAttackingPlayer;

    private const string WITCH_BOX_COLLIDER_NAME = "WitchBox";

    [Header("Raycast")]
    [SerializeField] protected float eyeLevel = 1.625f;
    [SerializeField] protected float sightDistance = 15f;
    [SerializeField] protected float fieldOfView = 90f;

    // Flying around.
    //[SerializeField] private WitchRoute witchRoute;
    private float routeTimer;
    private float routeInterval = 6f;
    private bool isInsideCollider;

    // Movement.
    [SerializeField] private float speed = 4.5f;
    [SerializeField] private float angularSpeed = 120f;
    [SerializeField] private CharacterController controller;

    private bool isMoving;
    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 route;

    // Attacks.
    private bool shield = true;
    [SerializeField] private Transform projectileSpawnPoint;
    private const string SKELETON_MAGIC_CHARGE = "SkeletonMagicCharge";
    private const float magicChargeSpeed = 10f;
    private float magicAttackInterval;
    private const float magicAttackIntervalMax = 3f;

    private void Awake()
    {
        health = MAX_HEALTH;
    }

    private void Update()
    {
        // TODO: Need to do proper check if the Witch is not in the collider.

        #region Testing ground movement.
        //if (Vector3.Distance(transform.position, route) < 0.2f || route == Vector3.zero)
        //{
        //    route = GetRandomGroundRoute();
        //}

        //SetGroundDestination(route);
        #endregion


        #region Testing flying.

        if (Vector3.Distance(transform.position, route) < 0.2f || route == Vector3.zero)
        {
            route = transform.position + UnityEngine.Random.insideUnitSphere * 5f;
        }

        FlyTo(route);

        #endregion

        magicAttackInterval += Time.deltaTime;
        if (CanSeePlayer())
        {
            AttackPlayer();
        }

        //Debug.Log(isInsideCollider);
    }

    protected override void TakeDamage(EnemyBase enemy, float damage)
    {
        if (enemy == this)
        {
            if (shield)
            {
                Debug.Log("The damage blocked by shield!");
                return;
            }

            health -= damage;
            Debug.Log(health);

            if (health <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void FlyTo(Vector3 destination)
    {
        //Debug.Log("Flying to " + route);
        CompletelyTurnOn(destination);
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }

    private void SetGroundDestination(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        controller.Move(direction * speed * Time.deltaTime);
        routeTimer = 0f;

        isMoving = true;

        velocity.y -= 9.8f * Time.deltaTime;
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -1f;
        }

        controller.Move(velocity * Time.deltaTime);
        LookTowards(destination);

        isGrounded = controller.isGrounded;
    }

    private Vector3 GetRandomGroundRoute()
    {
        Vector3 randomPoint = transform.position + (UnityEngine.Random.insideUnitSphere * 5f);
        randomPoint.y = transform.position.y;

        return randomPoint;
    }

    public override bool IsMoving()
    {
        return isMoving;
    }

   private void AttackPlayer()
    {
        //LookTowards(playerTransform.position);
        CompletelyTurnOn(playerTransform.position);

        if (magicAttackInterval <= magicAttackIntervalMax) return;
        else
        {
            OnAttackingPlayer?.Invoke();

            magicAttackInterval = 0;
        }
    }

    public void ShootProjectile()
    {
        Vector3 directionToPlayer = GetNormalizedDirectionTo(playerTransform.position);
        Vector3 chargeDirection = playerBody.position - projectileSpawnPoint.position;
        Quaternion rotation = Quaternion.LookRotation(chargeDirection);

        GameObject projectile;

        /* 3f - Mistakes in archer's aim.
            * 40f - Force added to velocity. */
        projectile = Instantiate(Resources.Load($"Prefabs/{SKELETON_MAGIC_CHARGE}") as GameObject, projectileSpawnPoint.position, rotation);
        projectile.GetComponent<Rigidbody>().velocity = directionToPlayer * magicChargeSpeed;

        //magicAttackInterval = 0;
    }

    //private void LookTowards(Vector3 point)
    //{
    //    Vector3 lookDirection = GetNormalizedDirectionTo(point);
    //    lookDirection.y = 0f;

    //    if (lookDirection != Vector3.zero)
    //    {
    //        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, angularSpeed * Time.deltaTime);
    //    }
    //}

    //private Vector3 GetNormalizedDirectionTo(Vector3 point)
    //{
    //    return (point - transform.position).normalized;
    //}


    private void CompletelyTurnOn(Vector3 position)
    {
        // Turn on smoothly.
        //Vector3 newRotation = Vector3.RotateTowards(transform.forward, position - transform.position, 1 * Time.deltaTime, 0.0f);
        //transform.rotation = Quaternion.LookRotation(newRotation);

        // Turn on instantly.
        Vector3 direction = (position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private bool CanSeePlayer() // Most likely won't be used in this class.
    {
        if (GetDistanceToPlayer() <= sightDistance)
        {
            Vector3 playerDirection = playerTransform.position - transform.position;
            if (Vector3.Angle(playerDirection, transform.forward) <= fieldOfView)
            {
                Debug.DrawRay(transform.position + Vector3.up * eyeLevel, playerDirection, Color.red);
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");

        if (other.gameObject.CompareTag(WITCH_BOX_COLLIDER_NAME))
        {
            isInsideCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");

        if (other.gameObject.CompareTag(WITCH_BOX_COLLIDER_NAME))
        {
            isInsideCollider = false;
        }
    }
}
