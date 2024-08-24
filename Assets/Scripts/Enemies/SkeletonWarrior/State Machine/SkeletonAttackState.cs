using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;
using static Skeleton;

public class SkeletonAttackState : SkeletonBaseState
{
    public event EventHandler OnAttackingPlayer;

    // Ranged skeleton.
    private float shotTimer;
    private float shotTimerMax = 3f; // Prev. value = 1.5f.

    private NavMeshAgent agent;
    private Transform playerTransform;

    private float attackDistance = 6f;

    public override void EnterState()
    {
        agent = skeleton.Agent;
        shotTimer = shotTimerMax;

        playerTransform = skeleton.GetPlayerTransform();

        if (skeleton.IsMeleeSkeleton)
            attackDistance = 2f;
        else
            attackDistance = 6f;
    }

    public override void PerformState()
    {
        if (skeleton.CanSeePlayer())
        {
            AttackPlayer();
        }

        else
        {
            stateMachine.SwitchState(stateMachine.searchState);
        }
    }

    // TODO: Move aside while attacking.
    private void AttackPlayer()
    {
        Vector3 directionToPlayer = playerTransform.position - skeleton.transform.position;
        Quaternion rotiationToPlayer = Quaternion.LookRotation(directionToPlayer.normalized);
        skeleton.gameObject.transform.rotation = Quaternion.RotateTowards(skeleton.transform.rotation, rotiationToPlayer, agent.angularSpeed * Time.deltaTime);

        if (skeleton.GetDistanceToPlayer() > attackDistance)
        {
            agent.SetDestination(playerTransform.position);
        }

        else
        {
            agent.ResetPath(); // To prevent a skeleton going to the character though the distance is normal.

            if (skeleton.IsMeleeSkeleton)
                // Melee skeletons' logic.
                skeleton.InvokeOnAttackingPlayerEvent();

            else
            {
                // Ranged skeletons' logic.
                if (skeleton.GetDistanceToPlayer() <= (0.5f * attackDistance))
                {
                    // To make distance between a skeleton and the character
                    agent.SetDestination(skeleton.transform.position - directionToPlayer);
                    skeleton.transform.rotation = Quaternion.RotateTowards(skeleton.transform.rotation, rotiationToPlayer, agent.angularSpeed * Time.deltaTime);
                }

                shotTimer += Time.deltaTime;

                if (shotTimer >= shotTimerMax)
                {
                    /* This invokes the event for SkeletonAnimations.cs that plays attacking animation.
                    * Then there's an animator event that calls DamageToPlayer() from CharacterAttributes.cs and causes damage to the character.*/
                    skeleton.InvokeOnAttackingPlayerEvent();

                    shotTimer = 0;
                }
            }
        }
    }

    public Vector3 GetPlayerLastPosition()
    {
        return playerTransform.position;
    }
}
