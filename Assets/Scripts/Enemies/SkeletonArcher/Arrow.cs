using System;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public static event Action<float> OnPlayerShot;

    private void OnCollisionEnter(Collision collision)
    {
        if (PlayerCombat.Instance.IsPlayerLayer(collision.gameObject.layer))
        {
            OnPlayerShot?.Invoke(Skeleton.ARROW_DAMAGE);
        }

        else if (PlayerCombat.Instance.IsEnemyLayer(collision.gameObject.layer))
        {
            Debug.Log("Skeleton hit by a magic charge.");
        }

        Destroy(gameObject);
    }
}
