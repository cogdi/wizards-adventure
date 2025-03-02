using UnityEngine;

public class WitchAnimations : MonoBehaviour
{
    public const string IS_MOVING = "IsMoving";

    [SerializeField] private Animator controller;
    private Witch witch;

    private void Start()
    {
        witch = GetComponent<Witch>();
    }

    private void Update()
    {
        controller.SetBool(IS_MOVING, witch.IsMoving());
    }
}
