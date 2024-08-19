using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMagicCharge : MonoBehaviour
{
    public static event Action<float> OnPlayerHit;

    private float time;

    private void Update()
    {
        time += Time.deltaTime;

        if (time > 1.25f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag(PlayerCombat.PLAYER_TAG))
        {
            //collision.gameObject.GetComponent<CharacterAttributes>().TakeDamage(Skeleton.MAGIC_DAMAGE);
            OnPlayerHit?.Invoke(Skeleton.MAGIC_DAMAGE);

        }

        // Experimental function.
        else if (PlayerCombat.Instance.IsEnemyLayer(collision.gameObject.layer))
        {
            //OnPlayerHit?.Invoke(this, EventArgs.Empty);
            Debug.Log("Skeleton hit by a magic charge.");
        }

        Destroy(gameObject);
    }
}
