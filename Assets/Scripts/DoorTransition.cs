using UnityEngine;

public class DoorTransition : MonoBehaviour
{
    public bool IsPlayerInsideTrigger { get => _isPlayerInsideTrigger; }
    private bool _isPlayerInsideTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerCombat.PLAYER_TAG))
            _isPlayerInsideTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PlayerCombat.PLAYER_TAG))
            _isPlayerInsideTrigger = false;
    }
}
