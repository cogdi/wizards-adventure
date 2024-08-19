using System;
using UnityEngine;

public class DoorAnimations : MonoBehaviour
{
    private Door door;
    private Animator animator;

    private const string IS_OPENING = "IsOpening";

    private void Awake()
    {
        door = GetComponent<Door>();
        animator = GetComponent<Animator>();
        animator.SetBool(IS_OPENING, false);

        door.OnDoorStateChanged += Door_OnDoorStateChanged;
    }

    private void Door_OnDoorStateChanged(bool isOpening)
    {
        Debug.Log("Door opens/closes");

        animator.SetBool(IS_OPENING, isOpening);
    }
}
