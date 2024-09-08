using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class ItemMonoBehaviourForPlayerHand : MonoBehaviour
{
    [SerializeField] private Transform handBone;  // The transform of the player's hand bone to follow
    private Transform itemTransform;

    private void Start()
    {
        itemTransform = transform;
    }

    private void Update()
    {
        if (handBone != null)
        {
            // Update the item's position and rotation to match the hand bone
            itemTransform.position = handBone.position;
            itemTransform.rotation = handBone.rotation;
        }
    }

    // This method is called when the item is parented to the player
    public void SetHandBone(Transform newHandBone)
    {
        handBone = newHandBone;
    }

    // This method is called when the item is dropped
    public void OnItemDropped()
    {
        // Clear the hand bone reference
        handBone = null;
    }
}
