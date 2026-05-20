using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class BarbarianLongDistanceState : BarbarianBaseState
{
    public static event Action OnStateChanged;
    public static event Action OnRockThrowned;

    // Throwing rocks.
    private const string BARBARIAN_ROCK = "Barbarian_Rock";
    private const float rockThrowingSpeed = 40f;
    private bool isRockThrowed;

    // Rush.
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask wallLayer;
    public static bool IsRushing { get; private set; }

    private bool isDestinationSet;
    private float regularSpeed = 1.75f;
    private float rushSpeed = 7f;
    private float rushTimer;
    private const float rushTimerMax = 4f;
    private bool lookingAtPlayerDirection;


    // Debug.
    private Barbarian barbarian;
    private NavMeshAgent agent;
    private Transform playerTransform;


    public override void EnterState()
    {
        barbarian = stateMachine.Barbarian;
        agent = stateMachine.Agent;
        playerTransform = PlayerCombat.Instance.transform;

        agent.areaMask = 1 << NavMesh.GetAreaFromName("Walkable");

        stateMachine.Barbarian.OnPlayerHitInRush += OnPlayerHitInRush;
        stateMachine.Barbarian.OnWallHitInRush += OnWallHitInRush;
    }
    
    public override void PerformState()
    {
        if (barbarian.GetDistanceToPlayer() <= Barbarian.MEDIUM_DISTANCE &&
        IsRushing == false)
        {
            // The distance got out of the boundaries of long-distance attacks.
            OnStateChanged?.Invoke();
        }

        if (!isRockThrowed)
        {
            OnRockThrowned?.Invoke();

            isRockThrowed = true;
        }

        else
        {
            Rush();
        }
    }

    private void Rush()
    {
        // agent.updateRotation = false;
        if (!IsRushing)
            barbarian.LookTowards(playerTransform.position);


        rushTimer += Time.deltaTime;
        
        if (rushTimer >= rushTimerMax)
        {
            if (!lookingAtPlayerDirection)
            {
                agent.updateRotation = false;
                barbarian.LookTowards(playerTransform.position);

                lookingAtPlayerDirection = true;
            }

            agent.speed = rushSpeed;
            IsRushing = true;

            if (NavMesh.SamplePosition(barbarian.transform.position + barbarian.transform.forward * 5f,
             out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                Debug.Log("Barbarian Rush: Navmesh ended");

                StopRushing();
            }
        }
    }

    public void StopRushing()
    {
        agent.ResetPath();
        agent.updateRotation = true;
        
        agent.speed = regularSpeed;
        IsRushing = false;
        lookingAtPlayerDirection = false;
        rushTimer = 0f;
    }

    private void OnPlayerHitInRush()
    {
        Debug.Log("Barbarian Rush: Hit player");
        StopRushing();
    }

    private void OnWallHitInRush()
    {
        Debug.Log("Barbarian Rush: Hit wall");
        StopRushing();
    }

    public override void ExitState()
    {
        StopRushing();
    }
}
