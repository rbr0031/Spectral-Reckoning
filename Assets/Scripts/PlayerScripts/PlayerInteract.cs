using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerInteract : NetworkBehaviour
{
    [SerializeField] private Transform _camTransform;
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask _interactableLayer;
    private PlayerUI _playerUI;
    private InputManager inputManager;
    
    void Start()
    {
        _camTransform = GetComponent<PlayerLook>()._camTransform;
        _playerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();
    }

    void Update()
    {
        if(!IsOwner)
            return;

        _playerUI.UpdateText(string.Empty);
        //create a ray at the center of the camera, shooting outwards.
        Ray ray = new Ray(_camTransform.position, _camTransform.forward);
        //Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);
        RaycastHit hitInfo; //variable to store our collision information.

        //shoot our raycast and store the information in hitInfo. Returns true if we hit something.
        if (Physics.Raycast(ray, out hitInfo, distance, _interactableLayer)) 
        {
            if(hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                _playerUI.UpdateText(interactable.promtMessage);
                if (inputManager.onFoot.Interact.triggered)
                {
                    interactable.BaseInteract();
                } else if (inputManager.onFoot.Drop.triggered)
                {
                    interactable.BaseDrop();

                }
            }
        } 
    }
}
