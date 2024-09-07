using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PickUpItem : Interactable
{
    [SerializeField] private GameObject item;
    private bool itemPickedUp;

    // This function is where we will design our interaction using code.
    protected override void Interact()
    {
        // Request the server to toggle the item state
        ToggleItemServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] // Allow any client to send the request
    private void ToggleItemServerRpc()
    {
        // Toggle the item state on the server
        itemPickedUp = !itemPickedUp;

        // Set the item to be active or inactive on the server
        item.SetActive(!itemPickedUp);

        // Call a ClientRpc to update all clients
        ToggleItemClientRpc(itemPickedUp);
    }

    [ClientRpc]
    private void ToggleItemClientRpc(bool isPickedUp)
    {
        // Update the item on all clients
        item.SetActive(!isPickedUp);
    }
}
