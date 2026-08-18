using System;
using UnityEngine;
using UnityEngine.AI;

public class BarbarianLongDistanceState : BarbarianBaseState
{
    public static event Action OnPlayerHitOnWall;

    public static event Action OnStateChanged;
    public static event Action OnHeadacheStarted;
    public static event Action OnHeadachePassed;
    public static event Action OnPickingUpStone;

    // Throwing rocks.
    private const string BARBARIAN_ROCK = "Barbarian_Rock";
    private const float rockThrowingSpeed = 40f;
    private bool isRockPickedUp;

    // Rush.
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask wallLayer;
    public static bool IsRushing { get; private set; }
    private bool isHeadaching;
    private float headacheTimer;
    private float headacheTimerMax = 5f;
    private bool isHoldingPlayer;

    private bool isDestinationSet;
    private float rushTimer;
    private const float rushTimerMax = 4f;
    private bool lookingAtPlayerDirection;


    // Debug.
    private Barbarian barbarian;
    private NavMeshAgent agent;
    private Transform playerTransform;

    private BarbarianObjectPool<BarbarianStoneProjectile> stonesPool;

    public override void EnterState()
    {
        barbarian = stateMachine.Barbarian;
        agent = stateMachine.Agent;
        playerTransform = PlayerCombat.Instance.transform;

        agent.areaMask = 1 << NavMesh.GetAreaFromName("Walkable");

        stateMachine.Barbarian.OnPlayerHitInRush += OnPlayerHitInRush;
        stateMachine.Barbarian.OnWallHitInRush += OnWallHitInRush;        
        stateMachine.Barbarian.OnStonesThrowned += Barbarian_OnStonesThrowned;        
    }

    private void Barbarian_OnStonesThrowned()
    {
        isRockPickedUp = false;

        // if (barbarian.Rocks.Count < 1)
        // {
        //     allRocksCrushed = true;
        // }
    }


    public override void PerformState()
    {
        if (isHeadaching)
        {
            ExperienceHeadache();
        }

        else
        {
            if (barbarian.GetDistanceToPlayer() <= Barbarian.MEDIUM_DISTANCE && !IsRushing)
            {
                // The distance got out of the boundaries of long-distance attacks.
                OnStateChanged?.Invoke();
            }

            //if (!allRocksCrushed)
            if (!barbarian.AllRocksCrushed)
            {
                if (!isRockPickedUp)
                {
                    PickUpRock();
                }
            }

            else
            {
                Rush();
            }
        }
    }

    private void ExperienceHeadache()
    {
        headacheTimer += Time.deltaTime;
        if (headacheTimer >= headacheTimerMax)
        {
            isHeadaching = false;
            OnHeadachePassed?.Invoke();
            headacheTimer = 0;
        }
    }


    private void PickUpRock()
    {
        agent.SetDestination(barbarian.Rocks[barbarian.Rocks.Count - 1].transform.position);
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            OnPickingUpStone?.Invoke();
            isRockPickedUp = true;
        }
    }


    private void Rush()
    {
        if (!IsRushing)
        {
            LookAtPlayer();
        }

        rushTimer += Time.deltaTime;
        
        if (rushTimer >= rushTimerMax)
        {
            if (!lookingAtPlayerDirection)
            {
                return;
            }

            barbarian.ApplyRushSpeed();
            IsRushing = true;

            if (NavMesh.SamplePosition(barbarian.transform.position + barbarian.transform.forward * 5f,
             out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                Debug.Log("Barbarian Rush: Navmesh ended");

                if (isHoldingPlayer)
                {
                    OnHitPlayerOnWall();
                    return;
                }

                StopRushing();

                isHeadaching = true;
                OnHeadacheStarted?.Invoke();
            }
        }
    }

    private void LookAtPlayer()
    {
        agent.updateRotation = false;
        barbarian.LookTowards(playerTransform.position);
        lookingAtPlayerDirection = true;
        agent.updateRotation = true;
    }

    public void StopRushing()
    {
        agent.ResetPath();
        agent.updateRotation = true;
        
        barbarian.ApplyRegularSpeed();
        IsRushing = false;
        lookingAtPlayerDirection = false;
        rushTimer = 0f;
    }

    private void OnPlayerHitInRush()
    {
        Debug.Log("Barbarian Rush: Hit player");
        
        PlayerMotor.Instance.DisableMovement();
        PlayerMotor.Instance.transform.SetParent(barbarian.transform);
        isHoldingPlayer = true;
    }
    
    private void OnWallHitInRush()
    {
        Debug.Log("Barbarian Rush: Hit wall");

        if (isHoldingPlayer)
        {
            OnHitPlayerOnWall();
            return;
        }

        StopRushing();

        isHeadaching = true;
        OnHeadacheStarted?.Invoke();
    }

    private void OnHitPlayerOnWall()
    {
        StopRushing();

        OnPlayerHitOnWall?.Invoke();

        PlayerMotor.Instance.transform.SetParent(null);
        PlayerMotor.Instance.EnableMovement();
        isHoldingPlayer = false;
    }

    public override void ExitState()
    {
        stateMachine.Barbarian.OnPlayerHitInRush -= OnPlayerHitInRush;
        stateMachine.Barbarian.OnWallHitInRush -= OnWallHitInRush;
        stateMachine.Barbarian.OnStonesThrowned -= Barbarian_OnStonesThrowned;        

        StopRushing();
    }
}
