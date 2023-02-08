using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PlayerAvatar : Biped
{
    [SerializeField] InputActionAsset inputAsset;

    [Space]
    [SerializeField] float mouseSensitivity;
    [SerializeField] float gamepadSensitivity;

    InputActionMap inputMap;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction faceAction;

    protected override void Awake()
    {
        inputAsset.Enable();
        inputMap = inputAsset.FindActionMap("Player");

        moveAction = inputMap.FindAction("Move");
        jumpAction = inputMap.FindAction("Jump");
        faceAction = inputMap.FindAction("FaceDirection");

        base.Awake();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    protected override void Update()
    {
        var moveInput = moveAction.ReadValue<Vector2>();
        Movement.MoveDirection = transform.TransformDirection(moveInput.x, 0.0f, moveInput.y);

        Movement.Jump = jumpAction.ReadValue<float>() > 0.5f;

        CalculateFaceDirection();

        base.Update();
    }

    private void CalculateFaceDirection()
    {
        var lookWithMouse = true;

        if (Gamepad.current != null)
        {
            if (Gamepad.current.lastUpdateTime < Mouse.current.lastUpdateTime)
            {
                lookWithMouse = false;
            }
        }

        Vector2 delta = Vector2.zero;
        if (lookWithMouse)
        {
            if (Mouse.current == null) return;
            delta = Mouse.current.delta.ReadValue() * mouseSensitivity;
        }
        else
        {
            delta = faceAction.ReadValue<Vector2>() * gamepadSensitivity * Time.deltaTime;
        }
        Movement.FaceRotation += delta;
    }
}
