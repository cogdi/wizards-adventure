using UnityEngine;

public class BarbarianStoneProjectile : MonoBehaviour
{
    private BarbarianObjectPool<BarbarianStoneProjectile> pool;

    public BarbarianStoneProjectile(BarbarianObjectPool<BarbarianStoneProjectile> poolRef)
    {
        pool = poolRef;
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO: Damage logic.

        pool.ReturnToPool(this);
    }
}
