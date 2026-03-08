using UnityEngine;

public class BarbarianStoneProjectile : MonoBehaviour
{
    private BarbarianObjectPool<BarbarianStoneProjectile> pool;

    public BarbarianStoneProjectile(BarbarianObjectPool<BarbarianStoneProjectile> poolRef)
    {
        /* To create a stone in the Barbarian's attack logic 
        I should create a pool then add the reference to it at each of the copies. */
        
        pool = poolRef;
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO: Damage logic.
        Debug.Log("Stone projectile OnTrggerEnter");

        pool.ReturnToPool(this);
    }
}
