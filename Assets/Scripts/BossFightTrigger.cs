using System;
using UnityEngine;

public class BossFightTrigger : MonoBehaviour
{
    public static event Action OnBossFightTriggered;
    public static bool IsBossFightTriggered = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerCombat.PLAYER_TAG))
        {
            IsBossFightTriggered = true;
            OnBossFightTriggered?.Invoke();
            Debug.Log("Collided with " + other.name);
        }
    }
}
