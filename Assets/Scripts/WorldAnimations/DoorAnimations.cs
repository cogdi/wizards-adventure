using UnityEngine;

public class DoorAnimations : MonoBehaviour
{
    [SerializeField] private Door door;
    [SerializeField] private Animator animator;

    private const string IS_OPENING = "IsOpening";

    private void Awake()
    {
        animator.SetBool(IS_OPENING, false);

        door.OnDoorStateChanged += Door_OnDoorStateChanged;
    }

    private void Door_OnDoorStateChanged(bool isOpening)
    {
        // When IS_OPENING is true - door opens.
        // When it's false - door closes.

        // ¯\_ಠ_ಠ_/¯ - Dana is the author of this comment.

        Debug.Log("Door opens/closes");

        animator.SetBool(IS_OPENING, isOpening);
    }
}
