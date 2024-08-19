using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public static event Action<float> OnPlayerShot;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag(PlayerCombat.PLAYER_TAG))
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
