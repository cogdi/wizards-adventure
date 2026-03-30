using UnityEngine;

public class HealingObject : MonoBehaviour
{
    public static System.Action<float> OnHealthRestored;

    private void OnTriggerEnter(Collider collider)
    {
        if (PlayerCombat.Instance.IsPlayerLayer(collider.gameObject.layer))
        {
            if (CharacterAttributes.Instance.Health < CharacterAttributes.MAX_HEALTH)
            {
                OnHealthRestored?.Invoke(20f);
                Destroy(gameObject);
            }
        }
    }
}
