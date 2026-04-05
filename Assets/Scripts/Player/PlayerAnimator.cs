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
    
    
    // private const string IS_WALKING = "IsWalking";
    // private const string IS_RUNNING = "IsRunning";
    // private const string IS_BLOCKING = "IsBlocking";
    // private const string IS_SPELLCASTING = "IsSpellcasting";
    // private const string TRIGGER_ATTACK = "Attack";
    // private const string TRIGGER_DODGE = "Dodge";
    

    private PlayerInput playerInputInstance;

    [SerializeField] private Animator animator;
    private float attackTimer;
    private float attackTimerMax = 1.5f;

    private void Start()
    {
        playerInputInstance = PlayerInput.Instance;

        attackTimer = attackTimerMax;

        PlayerCombat.Instance.OnChargingMagicAttack += HandleSpellcasting;
        //PlayerInput.Instance.OnDodgePerformed += HandleDodging;
        PlayerMotor.Instance.OnPlayerDodged += HandleDodging;
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

    // private void HandleDodging()
    // {
    //     animator.SetTrigger(DodgeLeft);
    // }
    
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
            default:
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

    private void HandleAttacking()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackTimerMax && playerInputInstance.IsAttackTriggered())
        {
            animator.SetTrigger(Attack);
            attackTimer = 0;
        }
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
