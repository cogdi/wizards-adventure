using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }

    public event Action OnInteractPerformed;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.OnFoot.Enable();

        playerInputActions.OnFoot.Interact.performed += Interact_performed;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractPerformed?.Invoke();
    }

    private void OnDestroy()
    {
        playerInputActions.Dispose();
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

    public bool IsAttackTriggered()
    {
        return playerInputActions.OnFoot.Attack.triggered;
    }

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
