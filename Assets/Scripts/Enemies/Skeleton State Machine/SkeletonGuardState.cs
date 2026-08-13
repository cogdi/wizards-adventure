using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonGuardState : SkeletonBaseState
{
    // This state is used both by melee and ranged skeletons.
    // The difference is that melee skeletons do patrolling and the others don't.

    private NavMeshAgent agent;
    private Transform guardPost;
    

    // They'll go to help when Boss-Fight started.
    private bool isAssistanceSkeleton;
    private Transform assistancePost;
    
    // To make the skeletons look to the right side on their guard posts.
    private bool isStayingOnPost;
    private bool isLookingRightWay;
    

    public override void EnterState()
    {
        agent = skeleton.Agent;
        isAssistanceSkeleton = skeleton.IsAssistanceSkeleton;
        assistancePost = skeleton.AssistancePosition;
        
        List<Transform> patrolPointList = skeleton.GetPatrolPointList();
        guardPost = patrolPointList[0];
        
        isLookingRightWay = false;

        SoundManager.Instance.OnAnySoundMade += PlayerCombat_OnWallHit;
        BossFightManager.OnBossFightTriggered += BossFightManager_OnBossFightTriggered; 
    }

    private void BossFightManager_OnBossFightTriggered()
    {
        if (isAssistanceSkeleton)
        {
            //SendAgentToHelp();
            SendAgentToPost(assistancePost);

        }
    }

    public override void PerformState()
    {
        if (skeleton.CanSeePlayer())
        {
            stateMachine.SwitchState(stateMachine.attackState);
        }

        else if (guardPost)
        {
            if (!isStayingOnPost && !isLookingRightWay)
            {
                //SendAgentToGuardPost();
                SendAgentToPost(guardPost);
            }
        }
    }

    private void SendAgentToPost(Transform post)
    {
        /* This method is mostly needed to make Skeletons look at the right directions when they come back
           to their posts. */
        
        agent.SetDestination(post.position);
   
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isStayingOnPost)
            {
                agent.ResetPath();
                isStayingOnPost = true;
            }
            
            skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, post.rotation, Time.deltaTime * 10f); // 10
        
            float dot = Quaternion.Dot(skeleton.transform.rotation.normalized, post.rotation.normalized);

            if (dot > 0.999f)
            {
                isLookingRightWay = true;
            }
        }
    }

    // private void SendAgentToGuardPost()
    // {
    //     /* This method is mostly needed to make Skeletons look at the right directions when they come back
    //        to their posts. */
        
    //     agent.SetDestination(guardPost.position);
   
    //     if (agent.remainingDistance <= agent.stoppingDistance)
    //     {
    //         if (!isStayingOnPost)
    //         {
    //             agent.ResetPath();
    //             isStayingOnPost = true;
    //         }
            
    //         skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, guardPost.rotation, Time.deltaTime * 10f); // 10
        
    //         float dot = Quaternion.Dot(skeleton.transform.rotation.normalized, guardPost.rotation.normalized);

    //         if (dot > 0.999f)
    //         {
    //             isLookingRightWay = true;
    //         }
    //     }
    // }

    // private void SendAgentToHelp()
    // {
    //     agent.SetDestination(assistancePosition.position);
   
    //     if (agent.remainingDistance <= agent.stoppingDistance)
    //     {
    //         if (!isStayingOnPost)
    //         {
    //             agent.ResetPath();
    //             isStayingOnPost = true;
    //         }
            
    //         skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, guardPost.rotation, Time.deltaTime * 10f); // 10
        
    //         float dot = Quaternion.Dot(skeleton.transform.rotation.normalized, guardPost.rotation.normalized);

    //         if (dot > 0.999f)
    //         {
    //             isLookingRightWay = true;
    //         }
    //     }
    // }

    private void PlayerCombat_OnWallHit(Vector3 hitPosition)
    {
        if (Vector3.Distance(skeleton.transform.position, hitPosition) <= 13f)
        {
            stateMachine.SwitchState(stateMachine.searchState);
        }
    }

    public override void UnsubscribeFromEvents()
    {
        SoundManager.Instance.OnAnySoundMade -= PlayerCombat_OnWallHit;
        BossFightManager.OnBossFightTriggered -= BossFightManager_OnBossFightTriggered;
    }

    public override void ExitState()
    {
        base.ExitState();
        isStayingOnPost = false;
        isLookingRightWay = false;
    }
}
