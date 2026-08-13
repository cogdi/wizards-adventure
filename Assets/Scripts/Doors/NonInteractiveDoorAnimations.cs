using UnityEngine;

public class NonInteractiveDoorAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    private void Start()
    {
        BossFightManager.OnBossFightTriggered += BossFightManager_OnBossFightTriggered;
    }

    private void BossFightManager_OnBossFightTriggered()
    {
        animator.SetTrigger("BossFightTrigger");
    }

    private void OnDestroy()
    {
        BossFightManager.OnBossFightTriggered -= BossFightManager_OnBossFightTriggered;
    }
}
