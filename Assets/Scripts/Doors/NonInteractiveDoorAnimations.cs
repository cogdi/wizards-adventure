using UnityEngine;

public class NonInteractiveDoorAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    private void Start()
    {
        BossFightTrigger.OnBossFightTriggered += BossFightTrigger_OnBossFightTriggered;
    }

    private void BossFightTrigger_OnBossFightTriggered()
    {
        animator.SetTrigger("BossFightTrigger");
    }
}
