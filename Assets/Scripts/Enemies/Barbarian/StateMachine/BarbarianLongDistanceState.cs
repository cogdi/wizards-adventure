using System;
using System.Collections;
using System.Collections.Generic;
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
    private bool isRushing;
    private bool isDestinationSet;
    private float regularSpeed = 1.75f;
    private float rushSpeed = 5f;
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
    }
    
    public override void PerformState()
    {
        if (barbarian.GetDistanceToPlayer() <= Barbarian.MEDIUM_DISTANCE)
        {
            // The distance got out of the boundaries of long-distance attacks.
            OnStateChanged?.Invoke();
        }

        if (!isRockThrowed)
        {
            //ShootProjectile();
            OnRockThrowned?.Invoke();

            isRockThrowed = true;
        }

        else
        {
            // Find an alternative way to start a coroutine, that doesn't need MonoBehaviour in this class.
            //StartCoroutine(Rush());
            Rush();
        }
    }
    
    // public void Rush() // IEnumerator
    // {
    //     if (!lookingAtPlayerDirection)
    //     {
    //         stateMachine.Barbarian.LookTowards(PlayerCombat.Instance.transform.position);
            
    //         //yield return new WaitForSeconds(2);

    //         lookingAtPlayerDirection = true;
    //     }

    //     rushTimer += Time.deltaTime;

    //     if (rushTimer >= rushTimerMax)
    //     {
    //         stateMachine.Agent.speed = rushSpeed;
    //         isRushing = true;

    //         // Точка впереди по направлению агента
    //         Vector3 targetPoint = stateMachine.Barbarian.transform.position + stateMachine.Barbarian.transform.forward * 5f;

    //         // Проверим, есть ли под ней NavMesh
    //         if (NavMesh.SamplePosition(targetPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
    //         {
    //             stateMachine.Agent.SetDestination(hit.position);
    //         }
    //         else
    //         {
    //             // Если NavMesh закончился — можно, например, остановиться
    //             Debug.Log("Navmesh ended");

    //             StopRushing();
    //         }
    //     }
    // }

    // public void StopRushing()
    // {
    //     stateMachine.Agent.ResetPath();
    //     stateMachine.Agent.speed = regularSpeed;
    //     isRushing = false;
    //     lookingAtPlayerDirection = false;
    //     rushTimer = 0f;
    // }

    public void Rush() // IEnumerator
    {
        if (!lookingAtPlayerDirection)
        {
            barbarian.LookTowards(playerTransform.position);
            
            // Vector3 direction = playerTransform.position - barbarian.transform.position; // направление к цели
            // direction.y = 0; // игнорируем высоту, чтобы смотреть только по горизонту
            // barbarian.transform.rotation = Quaternion.LookRotation(direction);

            //yield return new WaitForSeconds(2);

            lookingAtPlayerDirection = true;
        }

        rushTimer += Time.deltaTime;
        // Debug.Log(rushTimer);
        if (rushTimer >= rushTimerMax)
        {
            // Debug.Log(rushTimer);

            agent.speed = rushSpeed;
            isRushing = true;

            // Точка впереди по направлению агента
            Vector3 targetPoint = barbarian.transform.position + barbarian.transform.forward * 5f;

            // Проверим, есть ли под ней NavMesh
            if (NavMesh.SamplePosition(targetPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                // Debug.Log(hit.position);
                agent.SetDestination(hit.position);
            }
            else
            {
                // Если NavMesh закончился — можно, например, остановиться
                Debug.Log("Navmesh ended");

                StopRushing();
            }
        }
    }

    public void StopRushing()
    {
        agent.ResetPath();
        agent.speed = regularSpeed;
        isRushing = false;
        lookingAtPlayerDirection = false;
        rushTimer = 0f;
    }

    public override void ExitState()
    {
        StopRushing();
    }



    // // TODO: Make access to this methods using EnemyBase class later.
    // /* Also cache some fields/properties of the states. */
    // private float angularSpeed = 120f;
    // protected void LookTowards(Vector3 point)
    // {
    //     Vector3 lookDirection = GetNormalizedDirectionTo(point);
    //     lookDirection.y = 0f;

    //     if (lookDirection != Vector3.zero)
    //     {
    //         Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
    //         stateMachine.Barbarian.transform.rotation = Quaternion.Slerp(stateMachine.Barbarian.transform.rotation, targetRotation, angularSpeed * Time.deltaTime);
    //     }
    // }

    // protected Vector3 GetNormalizedDirectionTo(Vector3 point)
    // {
    //     return (point - stateMachine.Barbarian.transform.position).normalized;
    // }
}
