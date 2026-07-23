using UnityEngine;

public class BarbarianStoneProjectile : MonoBehaviour
{
    private BarbarianObjectPool<BarbarianStoneProjectile> pool;
    private float flyTime = 0f;
    private const float FLY_TIME_MAX = 3.5f;
    private Rigidbody rb;

    public BarbarianStoneProjectile(BarbarianObjectPool<BarbarianStoneProjectile> poolRef)
    {
        /* To create a stone in the Barbarian's attack logic 
        I should create a pool then add the reference to it at each of the copies. */
        pool = poolRef;        
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO: Damage logic.
        Debug.Log("Stone projectile OnTriggerEnter");

        if (other.CompareTag(PlayerCombat.PLAYER_TAG))
            pool.ReturnToPool(this);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();       
    }

    public void SetPoolReference(BarbarianObjectPool<BarbarianStoneProjectile> poolRef)
    {
        pool = poolRef;
    }

    private void Update()
    {
        flyTime += Time.deltaTime;
        if (flyTime >= FLY_TIME_MAX)
        {
            flyTime = 0;
            pool.ReturnToPool(this);
        }
    }

    public void ShootProjectile(Vector3 direction, float speed)
    {
        rb.velocity = direction * speed;
    }
}
