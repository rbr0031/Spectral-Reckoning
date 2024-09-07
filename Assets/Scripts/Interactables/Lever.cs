using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Lever : Interactable
{
    [SerializeField] private GameObject door;
    private bool doorOpen;

    // This function is where we will design our interaction using code.
    protected override void Interact()
    {
        // Request the server to toggle the door state
        ToggleDoorServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] // Allow any client to send the request
    private void ToggleDoorServerRpc()
    {
        // Toggle the door state on the server
        doorOpen = !doorOpen;

        // Set the animation on the server
        door.GetComponent<Animator>().SetBool("IsOpen", doorOpen);

        // Call a ClientRpc to update all clients
        ToggleDoorClientRpc(doorOpen);
    }

    [ClientRpc]
    private void ToggleDoorClientRpc(bool isOpen)
    {
        // Update the animation on all clients
        door.GetComponent<Animator>().SetBool("IsOpen", isOpen);
    }
}
