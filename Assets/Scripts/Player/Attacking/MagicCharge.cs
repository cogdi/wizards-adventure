using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCharge : MonoBehaviour
{
    // This GO can have one of these three tags: WizardLightMagicCharge, WizardMediumMagicCharge, WizardStrongMagicCharge.

    public class OnAnyHitEventArgs : EventArgs
    {
        public Vector3 hitPosition;
    }

    public class OnSkeletonDamagedEventArgs : EventArgs
    {
        public Skeleton skeleton;
        public float damage;
    }

    // TODO: Should it be static? Try to shoot one charge and immediately second, check if this will provide double damage.
    public static event EventHandler<OnAnyHitEventArgs> OnWallHit;
    public static event EventHandler<OnAnyHitEventArgs> OnSkeletonHit;
    public static event EventHandler<OnSkeletonDamagedEventArgs> OnSkeletonDamaged;

    private OnAnyHitEventArgs onAnyHitEventArgs = new OnAnyHitEventArgs();
    private OnSkeletonDamagedEventArgs onSkeletonDamagedEventArgs = new OnSkeletonDamagedEventArgs();
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
        onAnyHitEventArgs.hitPosition = collision.collider.ClosestPointOnBounds(transform.position);

        if (PlayerCombat.Instance.IsEnemyLayer(collision.gameObject.layer))
        {
            // TODO: Make a better if-check in case there're more enemies added to the game.

            OnSkeletonHit?.Invoke(this, onAnyHitEventArgs);

            onSkeletonDamagedEventArgs.skeleton = collision.gameObject.GetComponent<Skeleton>();
            onSkeletonDamagedEventArgs.damage = PlayerCombat.Instance.MagicAttacksDamageDictionary[tag];
            OnSkeletonDamaged?.Invoke(this, onSkeletonDamagedEventArgs);
        }

        else
        {
            OnWallHit?.Invoke(this, onAnyHitEventArgs);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        //OnWallHit?.Invoke(transform.position);
    }
}
