using System;
using UnityEngine;
using UnityEngine.AI;

public class Barbarian : MonoBehaviour
{
    public static Barbarian Instance { get; private set; }
    private BarbarianAnimations barbarianAnimationsInstance;
    public event Action<float> OnDamagingPlayer;

    [SerializeField] private NavMeshAgent agent;
    private float patrolTime = 0f;


    // TODO: Separate this aspects. Beta.
    //private Animator animator;
    //private const string IS_WALKING = "IsWalking";
    //private const string IS_TRIGGERED = "IsTriggered";
    //private bool isAttackingAnimationPlaying;

    private float eyeLevel = 1.15f; // 2f?
    private float sightDistance = 30f;
    private float fieldOfView = 100f;
    //private QueryTriggerInteraction ignoreRaycastMask;

    private const float MAX_HEALTH = 300f;
    private float health = MAX_HEALTH;
    private float meleeDamage = 15f;

    private Transform player;

    private void Awake()
    {
        Instance = this;
        //agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        player = PlayerCombat.Instance.gameObject.transform;
        barbarianAnimationsInstance = BarbarianAnimations.Instance;
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            AttackPlayer();
        }
     
        else Patrol();

        //Debug.Log(CanSeePlayer());
    }

    

    public bool IsWalking()
    {
        return agent.velocity.magnitude > 0.1f;
    }

    private void Patrol()
    {
        patrolTime += Time.deltaTime;

        if (patrolTime >= 5f)
        {
            agent.SetDestination(transform.position + UnityEngine.Random.insideUnitSphere * 5f);
            patrolTime = 0f;
        }
    }


    public bool CanSeePlayer()
    {
        // Debug.
        Ray ray = new Ray(transform.position + (Vector3.up * eyeLevel), transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 20f, Color.red);

        Vector3 playerDirection1 = player.transform.position - transform.position;
        Ray ray2 = new Ray(transform.position + (Vector3.up * eyeLevel), playerDirection1);
        Debug.DrawRay(ray2.origin, ray2.direction * Vector3.Distance(ray.origin, ray.direction), Color.green);


        if (GetDistanceToPlayer() <= sightDistance)
        {
            Vector3 playerDirection = player.transform.position - transform.position;
            if (Vector3.Angle(playerDirection, transform.forward) <= fieldOfView)
            {
                if (Physics.Raycast(transform.position + (Vector3.up * eyeLevel), playerDirection, out RaycastHit hitInfo, sightDistance))
                {
                    //Debug.Log(hitInfo.transform);
                    if (hitInfo.transform.CompareTag(PlayerCombat.PLAYER_TAG))//(hitInfo.transform == player.transform)
                    {
                        //playerLastPosition = player.transform.position;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    
   

    private void AttackPlayer()
    {
        //agent.autoRepath // TODO: Check out what is this and other fields and methods of NavMeshAgent.

        if (!barbarianAnimationsInstance.IsAttackingAnimationPlaying)
        {
            if (GetDistanceToPlayer() >= 4f)
            {
                agent.SetDestination(player.transform.position);
            }

            else
            {
                agent.ResetPath();
                barbarianAnimationsInstance.TriggerAttackingPlayer();
            }
        }
    }

    public void DamagePlayer()
    {
        OnDamagingPlayer?.Invoke(meleeDamage);
    }

    public void TakeDamage(float damage)
    {
        if (health - damage > 0)
            health -= damage;
        else Destroy(gameObject);
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(player.transform.position, transform.position);
    }
}