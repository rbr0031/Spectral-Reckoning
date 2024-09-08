using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PickUpItem : Interactable
{
    [SerializeField] private GameObject item;
    private bool itemPickedUp;
    private ulong currentHolderClientId; // To track which player is holding the item

    protected override void Interact()
    {
        // Prevent interaction if the item is already picked up by another player
        if (itemPickedUp && currentHolderClientId != NetworkManager.Singleton.LocalClientId)
        {
            return; // Only allow interaction if the item is not picked up or the current player owns it
        }

        // Get the player's NetworkObject and request the server to parent the item
        NetworkObject playerNetworkObject = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (playerNetworkObject != null)
        {
            ToggleItemServerRpc(playerNetworkObject);
        }
    }

    protected override void Drop()
    {
        // Prevent interaction if the item is already picked up by another player
        if (itemPickedUp && currentHolderClientId != NetworkManager.Singleton.LocalClientId)
        {
            
            return; // Only allow interaction if the item is not picked up or the current player owns it
        }

        DropItemServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] // Allow any client to send the request
    private void ToggleItemServerRpc(NetworkObjectReference playerNetworkObjectRef)
    {
        // Toggle the item state on the server
        itemPickedUp = true;

        if (itemPickedUp)
        {
            // Set the current holder's client ID
            if (playerNetworkObjectRef.TryGet(out NetworkObject playerNetworkObject))
            {
                currentHolderClientId = playerNetworkObject.OwnerClientId;

                // Transfer ownership and parent the item to the player
                item.GetComponent<NetworkObject>().ChangeOwnership(currentHolderClientId);
                item.GetComponent<NetworkObject>().TrySetParent(playerNetworkObject.transform);

                // Find the hand bone and attach the item
                Transform handBone = playerNetworkObject.transform.Find("CameraSocket/Hand");
                item.GetComponent<ItemMonoBehaviourForPlayerHand>().SetHandBone(handBone);
            }
        }

        // Synchronize the change to all clients
        ToggleItemClientRpc(itemPickedUp);
    }

    [ServerRpc(RequireOwnership = false)] // Allow any client to send the request
    private void DropItemServerRpc()
    {
        // Toggle the item state on the server
        itemPickedUp = false;

        // Clear the current holder's client ID
        currentHolderClientId = 0;

        // Unparent the item from the player
        item.GetComponent<NetworkObject>().TryRemoveParent();

        // Clear the hand bone reference
        item.GetComponent<ItemMonoBehaviourForPlayerHand>().OnItemDropped();

        // Synchronize the change to all clients
        ToggleItemClientRpc(itemPickedUp);
    }

    [ClientRpc]
    private void ToggleItemClientRpc(bool isPickedUp)
    {
        // Handle client-side logic for visual feedback or other updates
    }
}

