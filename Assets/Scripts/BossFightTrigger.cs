using System;
using UnityEngine;

public class BossFightTrigger : MonoBehaviour
{
    public event Action<BossFightManager.Boss> OnBossFightTriggered;
    public static bool IsBossFightTriggered;
    [SerializeField] private BossFightManager.Boss FightBoss;


    private void OnTriggerEnter(Collider other)
    {
        if (!IsBossFightTriggered)
        {
            if (other.CompareTag(PlayerCombat.PLAYER_TAG))
            {
                IsBossFightTriggered = true;
                OnBossFightTriggered?.Invoke(FightBoss);
            }
        }
    }
}
