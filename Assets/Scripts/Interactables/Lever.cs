using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Lever : Interactable
{
    [SerializeField]private GameObject door;
    private bool doorOpen;

    //this function is where we will design our interaction using code.
    protected override void Interact()
    {
        doorOpen = !doorOpen;
        door.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
    }
}
