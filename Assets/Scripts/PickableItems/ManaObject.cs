using UnityEngine;

public class ManaObject : MonoBehaviour
{
    public static System.Action<float> OnManaRestored;

    private void OnTriggerEnter(Collider collider)
    {
        if (PlayerCombat.Instance.IsPlayerLayer(collider.gameObject.layer))
        {
            if (CharacterAttributes.Instance.Mana < CharacterAttributes.MAX_MANA)
            {
                OnManaRestored?.Invoke(10f);
                Destroy(gameObject);
            }
        }
    }
}
