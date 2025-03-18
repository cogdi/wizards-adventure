using System;
using UnityEngine;

public class Witch : EnemyBase
{
    public event Action OnAttackingPlayer;

    [Header("Raycast")]
    [SerializeField] protected float eyeLevel = 1.625f;
    [SerializeField] protected float sightDistance = 15f;
    [SerializeField] protected float fieldOfView = 90f;

    // Flying around.
    [SerializeField] private WitchRoute witchRoute;
    private float routeTimer;
    private float routeInterval = 5f;
    private Vector3 currentRoute;

    // Movement.
    [SerializeField] private float speed = 4.5f;
    [SerializeField] private float angularSpeed = 120f;
    [SerializeField] private CharacterController controller;
    private bool isMoving;
    private bool isFlying;
    private bool isInsideTrigger; // not realised yet.
    private Vector3 velocity;
    private bool isGrounded;

    // Attacks.
    private bool shield = true;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Transform playerBody;
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

        //LookAt(playerTransform.position);
        //FlyToRandomPoint();
        //SetRandomGroundDestination();

        //SetDestination(transform.position + Random.insideUnitSphere * 5f);

        //Debug.Log("IsMoving = " + IsMoving());
        //Debug.Log("IsGrounded = " + isGrounded);
        //Debug.Log("Can see player = " + CanSeePlayer());

        magicAttackInterval += Time.deltaTime;
        if (CanSeePlayer())
        {
            AttackPlayer();
        }
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

    public override bool IsMoving()
    {
        return isMoving;
    }

    private void FlyToRandomPoint()
    {
        isMoving = true;

        if (currentRoute != Vector3.zero)
        {

            if (!IsRouteComplete(currentRoute))
            {
                isMoving = true;

                SetDestination(currentRoute);
            }

            else
            {
                isMoving = false;

                routeTimer += Time.deltaTime;
                if (routeTimer >= routeInterval)
                {
                    currentRoute = Vector3.zero;
                    routeTimer = 0;
                }
            }
        }

        else currentRoute = witchRoute.GetRandomPointInsideCollider();
    }

    private void SetDestination(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
    }

    private void SetRandomGroundDestination()
    {
        if (currentRoute != Vector3.zero)
        {
            if (!IsRouteComplete(currentRoute))
            {
                Vector3 direction = (currentRoute - transform.position).normalized;
                controller.Move(direction * speed * Time.deltaTime);

                isMoving = true;


                velocity.y -= 9.8f * Time.deltaTime;

                if (isGrounded && velocity.y < 0f)
                {
                    velocity.y = -1f;
                }

                controller.Move(velocity * Time.deltaTime);


                LookTowards(currentRoute);

                isGrounded = controller.isGrounded;
            }

            else
            {
                isMoving = false;

                routeTimer += Time.deltaTime;
                if (routeTimer >= routeInterval)
                {
                    currentRoute = Vector3.zero;
                    routeTimer = 0;
                }
            }
        }

        else currentRoute = GetRandomGroundPoint();
    }

   private void AttackPlayer()
    {
        LookTowards(playerTransform.position);

        if (magicAttackInterval <= magicAttackIntervalMax) return;
        else
        {
            Debug.Log("OnAttackingPlayer invoked with timer = " + magicAttackInterval);

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

    private void LookTowards(Vector3 point)
    {
        Vector3 lookDirection = GetNormalizedDirectionTo(point);
        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, angularSpeed * Time.deltaTime);
        }
    }

    private Vector3 GetNormalizedDirectionTo(Vector3 point)
    {
        return (point - transform.position).normalized;
    }

    private Vector3 GetRandomGroundPoint()
    {
        Vector3 randomPoint = transform.position + (UnityEngine.Random.insideUnitSphere * 2f);
        randomPoint.y = transform.position.y;

        return randomPoint;
    }

    private void CompletelyTurnOn(Vector3 position)
    {
        Vector3 newRotation = Vector3.RotateTowards(transform.forward, position - transform.position, 1 * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newRotation);
    }

    private bool IsRouteComplete(Vector3 routeEnd)
    {
        return Vector3.Distance(transform.position, routeEnd) < 0.2f;
    }

    private bool CanSeePlayer()
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

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.Equals(witchRoute))
        {
            isInsideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.Equals(witchRoute))
        {
            isInsideTrigger = false;
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Floor"))
        {
            //GetComponent<Rigidbody>().usegra
        }
    }
}
