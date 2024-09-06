using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Interactable : NetworkBehaviour
{
    //message displayed to player when lookin at an interactable
    public string promtMessage;

    //this function will be called from our player
    public void BaseInteract()
    {
        Interact();
    }

    protected virtual void Interact()
    {
        //no code written in this function
        //template to override in child classes
    }
}
