using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IS_RUNNING = "IsRunning";
    private const string IS_BLOCKING = "IsBlocking";
    private const string IS_SPELLCASTING = "IsSpellcasting";
    private const string TRIGGER_ATTACK = "Attack";

    private PlayerInput playerInputInstance;

    private Animator animator;

    private float attackTimer;
    private float attackTimerMax = 1.5f;

    private void Start()
    {
        playerInputInstance = PlayerInput.Instance;

        animator = GetComponent<Animator>();
        attackTimer = attackTimerMax;

        PlayerCombat.Instance.OnChargingMagicAttack += HandleSpellcasting;
    }

    private void Update()
    {
        HandleWalking();
        HandleRunning();
        HandleBlocking();

        if (!playerInputInstance.IsBlockingPressed())
        {
            HandleAttacking();
        }
    }

    private void HandleSpellcasting(bool isSpellcasting)
    {
        // // There's animation event inside Spellcasting animation, that triggers attack.
        animator.SetBool(IS_SPELLCASTING, isSpellcasting);
    }

    private void HandleWalking()
    {
        animator.SetBool(IS_WALKING, playerInputInstance.GetMovementVectorNormalized() != Vector2.zero);
    }

    private void HandleRunning()
    {
        animator.SetBool(IS_RUNNING, playerInputInstance.IsRunningTriggered());
    }

    private void HandleAttacking()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackTimerMax && playerInputInstance.IsAttackTriggered())
        {
            animator.SetTrigger(TRIGGER_ATTACK);
            attackTimer = 0;
        }
    }

    private void HandleBlocking()
    {
        animator.SetBool(IS_BLOCKING, playerInputInstance.IsBlockingPressed());
    }

    private void OnDestroy()
    {
        PlayerCombat.Instance.OnChargingMagicAttack -= HandleSpellcasting;
    }
}
