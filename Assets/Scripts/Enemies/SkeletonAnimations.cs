using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonAnimations : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string TRIGGER_ATTACK = "Attack";

    private Animator animator;
    private Skeleton skeleton;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        skeleton = animator.GetComponent<Skeleton>();

        animator.SetBool(IS_WALKING, false);
    }
    private void Start()
    {
        skeleton.OnAttackingPlayer += Skeleton_OnAttackingPlayer;
    }

    private void Skeleton_OnAttackingPlayer()
    {
        /* There's an animator event that triggers shooting
         * or DamageToPlayer() directly, in case if it's melee enemy. */
        animator.SetTrigger(TRIGGER_ATTACK);
    }

    private void Update()
    {
        HandleWalking();
    }

    private void HandleWalking()
    {
        if (skeleton.IsMoving())
        {
            animator.SetBool(IS_WALKING, true);
        }

        else
        {
            animator.SetBool(IS_WALKING, false);
        }
    }
}
