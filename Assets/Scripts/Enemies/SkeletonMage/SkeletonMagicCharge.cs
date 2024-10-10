using System;
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
        if (PlayerCombat.Instance.IsPlayerLayer(collision.gameObject.layer))
        {
            OnPlayerHit?.Invoke(Skeleton.MAGIC_DAMAGE);
        }

        // TODO: Do something with this experimental function.
        else if (PlayerCombat.Instance.IsEnemyLayer(collision.gameObject.layer))
        {
            Debug.Log("Skeleton hit by a magic charge.");
        }

        Destroy(gameObject);
    }
}
