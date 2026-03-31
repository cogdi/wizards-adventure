using System;
using UnityEngine;

public class MagicCharge : MonoBehaviour
{
    // This GO shall have one of these three tags: WizardLightMagicCharge, WizardMediumMagicCharge, WizardStrongMagicCharge.

    public static event Action<Vector3> OnWallHit;
    public static event Action<Vector3> OnEnemyHit;
    public static event Action<EnemyBase, float> OnEnemyDamaged;

    private float flyTime;

    private void Update()
    {
        flyTime += Time.deltaTime;

        if (flyTime > 2f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (PlayerCombat.Instance.IsEnemyLayer(collision.gameObject.layer))
        {
            OnEnemyHit?.Invoke(collision.collider.ClosestPointOnBounds(transform.position));
            OnEnemyDamaged?.Invoke(collision.gameObject.GetComponent<EnemyBase>(), PlayerCombat.Instance.MagicAttacksDamageDictionary[tag]);
        }

        else
        {
            OnWallHit?.Invoke(collision.collider.ClosestPointOnBounds(transform.position));
        }

        Destroy(gameObject);
    }
}
