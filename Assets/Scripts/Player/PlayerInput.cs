using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }

    public event Action OnJumpPerformed;
    public event Action OnInteractPerformed;
    public event Action OnDodgePerformed;
    public event Action OnMeleeAttackPerformed;
    

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        playerInputActions = new PlayerInputActions();
        playerInputActions.OnFoot.Enable();

        playerInputActions.OnFoot.Interact.performed += Interact_performed;
        playerInputActions.OnFoot.Jump.performed += Jump_performed;
        playerInputActions.OnFoot.Dodge.performed += Dodge_performed;
        playerInputActions.OnFoot.Attack.performed += Attack_performed;
    }

    private void Attack_performed(InputAction.CallbackContext obj)
    {
        OnMeleeAttackPerformed?.Invoke();
    }

    private void Dodge_performed(InputAction.CallbackContext obj)
    {
        OnDodgePerformed?.Invoke();
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        OnJumpPerformed?.Invoke();
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractPerformed?.Invoke();
    }

    private void OnDestroy()
    {
        playerInputActions.Dispose();
    }

    public bool IsFlyingPressed()
    {
        return playerInputActions.OnFoot.Fly.IsPressed();
    }

    public bool IsBlockingPressed()
    {
        return playerInputActions.OnFoot.Block.IsPressed();
    }

    public bool IsJumpTriggered()
    {
        return playerInputActions.OnFoot.Jump.triggered;
    }

    public bool IsRunningTriggered()
    {
        return playerInputActions.OnFoot.Run.ReadValue<float>() > 0f;
    }

    // public bool IsAttackTriggered()
    // {
    //     return playerInputActions.OnFoot.Attack.triggered;
    // }

    public bool IsMagicAttackTriggered()
    {
        return playerInputActions.OnFoot.MagicAttack.ReadValue<float>() > 0f;
    }

    public Vector2 GetMovementVectorNormalized()
    {
        return playerInputActions.OnFoot.Movement.ReadValue<Vector2>().normalized;
    }

    public Vector2 GetLookingAxis()
    {
        return playerInputActions.OnFoot.Look.ReadValue<Vector2>();
    }
}
