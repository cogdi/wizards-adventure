using System;
using UnityEngine;

public class MagicCharge : MonoBehaviour
{
    // This GO can have one of these three tags: WizardLightMagicCharge, WizardMediumMagicCharge, WizardStrongMagicCharge.

    // TODO: Should it be static? Try to shoot one charge and immediately second, check if this will provide double damage.
    public static event Action<Vector3> OnWallHit;
    public static event Action<Vector3> OnSkeletonHit;
    public static event Action<Skeleton, float> OnSkeletonDamaged;

    private float time;

    private void Update()
    {
        time += Time.deltaTime;

        if (time > 2f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (PlayerCombat.Instance.IsEnemyLayer(collision.gameObject.layer))
        {
            // TODO: Make a better if-check in case there're more enemies added to the game.
            OnSkeletonHit?.Invoke(collision.collider.ClosestPointOnBounds(transform.position));
            OnSkeletonDamaged?.Invoke(collision.gameObject.GetComponent<Skeleton>(), PlayerCombat.Instance.MagicAttacksDamageDictionary[tag]);
        }

        else
        {
            OnWallHit?.Invoke(collision.collider.ClosestPointOnBounds(transform.position));
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        //OnWallHit?.Invoke(transform.position);
    }
}
