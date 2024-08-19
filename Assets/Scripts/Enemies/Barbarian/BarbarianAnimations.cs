using System.Collections;
using System.Collections.Generic;
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
        Instance = this;

        animator.SetBool(IS_WALKING, false);
    }

    private void Start()
    {
        //barbarian.OnAttackingPlayer += Barbarian_OnAttackingPlayer;
    }

    private void Update()
    {
        if (!IsAttackingAnimationPlaying)
            HandleWalking();
    }

    private void HandleWalking()
    {
        animator.SetBool(IS_WALKING, barbarian.IsWalking());
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
