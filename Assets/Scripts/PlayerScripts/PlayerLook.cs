using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class PlayerLook : NetworkBehaviour
{
    [SerializeField] public Transform _camTransform;
    private float xRotation = 0f;
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;
    
    public override void OnNetworkSpawn()
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = _camTransform.GetComponent<CinemachineVirtualCamera>();

        if(IsOwner)
            cinemachineVirtualCamera.Priority = 1;
        else 
            cinemachineVirtualCamera.Priority = 0;
    }

    private void Awake() 
    {
        if(_camTransform == null)
            _camTransform = FindObjectOfType<CinemachineVirtualCamera>().transform;
    }

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        //calculate camera rotation for looking up and down
        xRotation -= mouseY * Time.deltaTime * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        //apply this to our camera transform.
        _camTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //rotate player to look left and right
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
    }
}
