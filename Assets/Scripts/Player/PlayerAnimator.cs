using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int IsBlocking = Animator.StringToHash("IsBlocking");
    private static readonly int IsSpellcasting = Animator.StringToHash("IsSpellcasting");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Dodge = Animator.StringToHash("Dodge");
    
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
        PlayerInput.Instance.OnDodgePerformed += HandleDodging;
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

    private void HandleDodging()
    {
        animator.SetTrigger(Dodge);
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
