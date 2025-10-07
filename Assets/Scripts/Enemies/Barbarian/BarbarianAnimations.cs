using UnityEngine;

public class BarbarianAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private Barbarian barbarian;

    private string ATTACK_TRIGGER = "Attack";

    private void Start()
    {
        barbarian = GetComponent<Barbarian>();
    }

    private void Update()
    {
        
    }
}
