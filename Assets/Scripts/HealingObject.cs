using UnityEngine;

public class HealingObject : MonoBehaviour
{
    public static System.Action<float> OnHealthRestored;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (CharacterAttributes.Instance.Health < CharacterAttributes.MAX_HEALTH)
            {
                OnHealthRestored?.Invoke(20f);
                Destroy(gameObject);
            }
        }
    }
}
