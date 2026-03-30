using System;
using UnityEngine;

public class BossFightTrigger : MonoBehaviour
{
    public static event Action OnBossFightTriggered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag(PlayerCombat.PLAYER_TAG))
        {
            OnBossFightTriggered?.Invoke();
            Debug.Log("Collided with " + other.name);
        }
    }
}
