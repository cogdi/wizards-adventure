using UnityEngine;

public class Witch : EnemyBase
{
    //public float Health => health;

    //protected const float MAX_HEALTH = 100f;

    //protected float health = 0f;
    //protected Transform playerTransform;
    //protected Vector3 playerLastPosition;
    //protected PlayerCombat playerCombatInstance;

    // Raycast
    protected float eyeLevel = 1.15f;
    protected float sightDistance = 15f;
    protected float fieldOfView = 100f;
    //protected int ignoreRaycastMask;

    // Flying around.
    [SerializeField] private WitchRoute witchRoute;
    private float routeTimer;
    private float routeInterval = 5f;
    private Vector3 currentRoute;

    // Movement.
    [SerializeField] private float speed = 4.5f;

    private bool isInsideTrigger; // not realised yet.

    private bool shield = true;

    private void Awake()
    {
        health = MAX_HEALTH;
    }

    //private void Start()
    //{
    //    playerCombatInstance = PlayerCombat.Instance;
    //    playerTransform = playerCombatInstance.transform;
    //    if (playerTransform == null)
    //    {
    //        Debug.LogError("Player instance can not be found.");
    //    }

    //    ignoreRaycastMask = ~LayerMask.GetMask("IgnoreSkeletonRaycast");

    //    PlayerCombat.Instance.OnEnemyDamaged += TakeDamage;
    //    MagicCharge.OnEnemyDamaged += TakeDamage;
    //}

    protected override void TakeDamage(EnemyBase enemy, float damage)
    {
        if (enemy == this)
        {
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
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        // TODO: Need to do proper check if the Witch is not in the collider.

        LookAt(playerTransform.position);
        GoToRandomPoint();
    }

    private void GoToRandomPoint()
    {
        if (currentRoute != Vector3.zero)
        {

            if (!IsRouteComplete(currentRoute))
            {
                SetDestination(currentRoute);
            }

            else
            {
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

    private void LookAt(Vector3 position)
    {
        Vector3 newRotation = Vector3.RotateTowards(transform.forward, position - transform.position, 1 * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newRotation);
    }

    private bool IsRouteComplete(Vector3 routeEnd)
    {
        return transform.position.Equals(routeEnd);
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

    //private void OnDestroy()
    //{
    //    PlayerCombat.Instance.OnEnemyDamaged -= TakeDamage;
    //    MagicCharge.OnEnemyDamaged -= TakeDamage;
    //}
}
