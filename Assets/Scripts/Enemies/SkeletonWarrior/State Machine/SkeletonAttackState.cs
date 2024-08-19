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

        if (skeleton.CompareTag(Skeleton.MELEE_SKELETON_TAG))
            /* Melee Skeleton stops 2f away from player when attacking.
             * Values different than 2f work poorly. */
            attackDistance = 2f;
        else
            /* Ranged Skeleton keeps 6f distance when shooting. */
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

    // TODO: Moving aside while attacking.
    //private void AttackPlayer()
    //{
    //    Vector3 directionToPlayer = playerTransform.position - skeleton.transform.position;
    //    Quaternion rotiationToPlayer = Quaternion.LookRotation(directionToPlayer.normalized);
    //    skeleton.gameObject.transform.rotation = Quaternion.RotateTowards(skeleton.transform.rotation, rotiationToPlayer, agent.angularSpeed * Time.deltaTime);

    //    if (skeleton.GetDistanceToPlayer() > (1.5 * attackDistance))
    //    {
    //        agent.SetDestination(playerTransform.position);
    //    }

    //    else
    //    {
    //        agent.ResetPath();

    //        if (skeleton.CompareTag(Skeleton.SKELETON_ARCHER_TAG) || skeleton.CompareTag(Skeleton.SKELETON_MAGE_TAG))
    //        {
    //            if (skeleton.GetDistanceToPlayer() < attackDistance)
    //            {
    //                // To make distance between a skeleton and the character
    //                agent.SetDestination(skeleton.transform.position - directionToPlayer);
    //                skeleton.transform.rotation = Quaternion.RotateTowards(skeleton.transform.rotation, rotiationToPlayer, agent.angularSpeed * Time.deltaTime);
    //            }

    //            shotTimer += Time.deltaTime;

    //            if (shotTimer >= shotTimerMax)
    //            {
    //                /* This invokes an event from SkeletonAnimations.cs that plays attacking animation.
    //                 * Then there's an animator event that calls DamageToPlayer() from CharacterAttributes.cs and causes damage to the character.*/
    //                skeleton.InvokeOnAttackingPlayerEvent();

    //                shotTimer = 0;
    //            }
    //        }

    //        else skeleton.InvokeOnAttackingPlayerEvent();
    //    }
    //}

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

            if (skeleton.CompareTag(MELEE_SKELETON_TAG))
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
                    /* This invokes an event from SkeletonAnimations.cs that plays attacking animation.
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
