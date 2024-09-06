using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class InputManager : NetworkBehaviour
{
    private PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;

    private PlayerMovement movement; 
    private PlayerLook look;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        movement = GetComponent<PlayerMovement>();
        look = GetComponent<PlayerLook>();

        onFoot.Jump.performed += ctx => movement.Jump();
        onFoot.Crouch.performed += ctx => movement.Crouch();
        onFoot.Sprint.performed += ctx => movement.Sprint();
    }

    private void FixedUpdate() 
    {
        if(!IsOwner) return;
        movement.ProcessMovement(onFoot.Movement.ReadValue<Vector2>());    
    }

    private void LateUpdate()
    {
        if(!IsOwner) return;
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());    
    }

    private void OnEnable() 
    {
        onFoot.Enable();
    }

    private void OnDisable() 
    {
        onFoot.Disable();
    }   
}
