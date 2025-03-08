using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Witch : EnemyBase
{
    // Raycast
    protected float eyeLevel = 1.15f;
    protected float sightDistance = 15f;
    protected float fieldOfView = 100f;

    // Flying around.
    [SerializeField] private WitchRoute witchRoute;
    private float routeTimer;
    private float routeInterval = 5f;
    private Vector3 currentRoute;

    // Movement.
    [SerializeField] private float speed = 4.5f;
    private bool isMoving;
    private bool isFlying;

    private bool isInsideTrigger; // not realised yet.

    private bool shield = true;

    [SerializeField] private CharacterController controller;

    private Vector3 velocity;
    private bool isGrounded;

    private void Awake()
    {
        health = MAX_HEALTH;
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

    private void Update()
    {
        // TODO: Need to do proper check if the Witch is not in the collider.

        //LookAt(playerTransform.position);
        //GoToRandomPoint();

        //SetDestination(transform.position + Random.insideUnitSphere * 5f);

        Debug.Log(IsMoving());
        SetRandomGroundDestination();
    }

    private void GoToRandomPoint()
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

    private void LookTowards(Vector3 point)
    {
        Vector3 lookDirection = (point - transform.position).normalized;
        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f * Time.deltaTime);
        }
    }

    private Vector3 GetRandomGroundPoint()
    {
        Vector3 randomPoint = transform.position + (Random.insideUnitSphere * 25f);
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
