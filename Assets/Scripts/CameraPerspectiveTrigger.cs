using System;
using UnityEngine;

public class CameraPerspectiveTrigger : MonoBehaviour
{
    public static event Action<bool> OnPlayerNearWall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerCombat.PLAYER_TAG))
        {
            OnPlayerNearWall?.Invoke(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PlayerCombat.PLAYER_TAG))
        {
            OnPlayerNearWall?.Invoke(false);
        }
    }
}
