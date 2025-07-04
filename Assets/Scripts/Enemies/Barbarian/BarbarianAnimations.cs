using UnityEngine;

public class BarbarianAnimations : MonoBehaviour
{
    public static BarbarianAnimations Instance { get; private set; }

    [SerializeField] private Animator animator;
    [SerializeField] private Barbarian barbarian;

    private const string IS_WALKING = "IsWalking";
    private const string IS_TRIGGERED = "IsTriggered";
    public bool IsAttackingAnimationPlaying { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        animator.SetBool(IS_WALKING, false);
    }

    private void Update()
    {
        if (!IsAttackingAnimationPlaying)
        {
            HandleWalking();
        }
    }

    private void HandleWalking()
    {
        animator.SetBool(IS_WALKING, barbarian.IsMoving());
    }

    public void ReversePlayingAttackingAnimationFlag()
    {
        // This is called from the beginning and the end of attacking animation.
        IsAttackingAnimationPlaying = !IsAttackingAnimationPlaying;
    }

    public void TriggerAttackingPlayer()
    {
        animator.SetTrigger(IS_TRIGGERED);
    }
}
