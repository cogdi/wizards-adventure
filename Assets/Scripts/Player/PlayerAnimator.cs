using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int IsBlocking = Animator.StringToHash("IsBlocking");
    private static readonly int IsSpellcasting = Animator.StringToHash("IsSpellcasting");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int DodgeLeft = Animator.StringToHash("DodgeLeft");
    private static readonly int DodgeRight = Animator.StringToHash("DodgeRight");
    private static readonly int DodgeForward = Animator.StringToHash("DodgeForward");
    private static readonly int DodgeBackward = Animator.StringToHash("DodgeBackward");

    private PlayerInput playerInputInstance;

    [SerializeField] private Animator animator;
    private float attackTimer;
    private float attackTimerMax = 0.5f;
    private int attacksCounter = 0;

    private void Start()
    {
        playerInputInstance = PlayerInput.Instance;

        attackTimer = attackTimerMax;

        PlayerCombat.Instance.OnChargingMagicAttack += HandleSpellcasting;
        PlayerMotor.Instance.OnPlayerDodged += HandleDodging;
        PlayerInput.Instance.OnMeleeAttackPerformed += HandleAttacks;
    }

    private void Update()
    {
        HandleWalking();
        HandleRunning();
        HandleBlocking();

        attackTimer += Time.deltaTime;
    }

    private Dictionary<int, string> AnimationIndexDictionary = new Dictionary<int, string>()
    { 
        { 0, "FirstAttack" },
        { 1, "SecondAttack" },
        { 2, "ThirdAttack" },
        { 3, "FourthAttack" }
    };

    private void HandleAttacks()
    {
        if (attacksCounter >= AnimationIndexDictionary.Count) attacksCounter = 0;

        if (!playerInputInstance.IsBlockingPressed() && attackTimer >= attackTimerMax)
        {
            animator.SetTrigger(AnimationIndexDictionary[attacksCounter]);

            attacksCounter++;
            attackTimer = 0;
        }
    }
    
    private void HandleDodging(PlayerMotor.DodgeTypes type)
    {
        switch (type)
        {
            case PlayerMotor.DodgeTypes.Forward: 
                Debug.Log("Forward dodge");
                animator.SetTrigger(DodgeForward);
                break;
            case PlayerMotor.DodgeTypes.Backward:
                Debug.Log("Backward dodge");
                animator.SetTrigger(DodgeBackward);
                break;
            case PlayerMotor.DodgeTypes.Right:
                Debug.Log("Right dodge");
                animator.SetTrigger(DodgeRight);
                break;
            case PlayerMotor.DodgeTypes.Left:
                Debug.Log("Left dodge");
                animator.SetTrigger(DodgeLeft);
                break;
        }
    }
    
    private void HandleSpellcasting(bool isSpellcasting)
    {
        // There's animation event inside Spellcasting animation, that triggers attack.
        animator.SetBool(IsSpellcasting, isSpellcasting);
    }

    private void HandleWalking()
    {
        animator.SetBool(IsWalking, playerInputInstance.GetMovementVectorNormalized() != Vector2.zero);
    }

    private void HandleRunning()
    {
        animator.SetBool(IsRunning, playerInputInstance.IsRunningTriggered());
    }
    
    private void HandleBlocking()
    {
        animator.SetBool(IsBlocking, playerInputInstance.IsBlockingPressed());
    }
    
    private void OnDestroy()
    {
        PlayerCombat.Instance.OnChargingMagicAttack -= HandleSpellcasting;
    }
}
