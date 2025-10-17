using System;
using UnityEngine;

public class BarbarianRock : MonoBehaviour
{
    public static event Action<float> OnPlayerHit;

    private void OnCollisionEnter(Collision collision)
    {
        if (PlayerCombat.Instance.IsPlayerLayer(collision.gameObject.layer))
        {
            OnPlayerHit?.Invoke(Barbarian.STONES_DAMAGE);
        }

        Destroy(gameObject);
    }
}
