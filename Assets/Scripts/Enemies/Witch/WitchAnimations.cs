using UnityEngine;

public class WitchAnimations : MonoBehaviour
{
    private const string IS_MOVING = "IsMoving";
    private const string ATTACK_TRIGGER = "Attack";

    [SerializeField] private Animator animator;
    private Witch witch;

    private void Start()
    {
        witch = GetComponent<Witch>();
        witch.OnAttackingPlayer += Witch_OnAttackingPlayer;
    }

    private void Witch_OnAttackingPlayer()
    {
        animator.SetTrigger(ATTACK_TRIGGER);
    }

    private void Update()
    {
        animator.SetBool(IS_MOVING, witch.IsMoving());
    }
}
