using UnityEngine;

public class Witch : Enemy
{
    private Vector3 playerPosition;

    [SerializeField] private WitchRoute witchRoute;

    private float routeTimer;
    private float routeInterval = 5f;

    private bool isInsideTrigger;

    private Vector3 currentRoute;

    private void OnEnable()
    {
        agent.speed = 2.5f;
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        //playerPosition = playerTransform.position;
        //agent.SetDestination(playerPosition);

        // TODO: Need to do proper check if the Witch is not in the collider.
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
        transform.position = Vector3.MoveTowards(transform.position, position, agent.speed * Time.deltaTime);
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
}
