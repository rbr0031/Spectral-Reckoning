using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerWeapon : NetworkBehaviour
{
    [SerializeField]private GameObject weapon;  // Reference to the weapon GameObject
    private InputManager inputManager;
    private bool isWeaponEquipped = true;  // Track if weapon is in hand
    [SerializeField] private GameObject bigDood; // Reference to the big Enemy GameObject
    [SerializeField] private GameObject jumpScare; // Reference to the jumpscare Enemy GameObject
    [SerializeField] private GameObject smallDood; // Reference to the small Enemy GameObject

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        if(weapon == null)
        {
            weapon = transform.Find("PlayerWeapon").gameObject;
        }
        if(smallDood == null)
        {
            smallDood = GameObject.Find("Enemy");
        }
        if(bigDood == null)
        {
            bigDood = GameObject.Find("BigDood");
        }
        if(jumpScare == null)
        {
            jumpScare = GameObject.Find("JumpScare");
        }
    }

    private void Update()
    {
        if (!IsOwner) return;  // Ensure only the owning player controls the weapon

        SwitchWeapon();
        WeaponAttack();


        //codigo temporario apenas para o trailer
        if (Input.GetKeyDown(KeyCode.Alpha2) && IsOwner)
        {
            RequestSetActiveServerRpc(false, "smallDood");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && IsOwner)
        {
            RequestSetActiveServerRpc(false, "bigDood");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && IsOwner)
        {
            RequestSetActiveServerRpc(true, "bigDood");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && IsOwner)
        {
            RequestSetActiveServerRpc(false, "jumpScare");
        }
    }

    private void SwitchWeapon()
    {
        if (inputManager.onFoot.Weapon.triggered)
        {
            if (isWeaponEquipped)
            {
                // Take weapon off the hand
                Debug.Log("Weapon holstered");
                isWeaponEquipped = false;
                weapon.SetActive(false);
                //weapon.GetComponent<Animator>().SetBool("IsEquipped", false);
            }
            else
            {
                // Put weapon in the hand
                Debug.Log("Weapon equipped");
                isWeaponEquipped = true;
                weapon.SetActive(true);
                //weapon.GetComponent<Animator>().SetBool("IsEquipped", true);
            }
        }
    }

    private void WeaponAttack()
    {
        if (isWeaponEquipped && inputManager.onFoot.WeaponAttack.triggered)
        {
            // Play particle effect for weapon attack
            Debug.Log("Weapon attack triggered");
            // You can instantiate or play a pre-configured particle effect here
            // Example:
            // ParticleSystem attackEffect = Instantiate(attackParticlePrefab, weaponMuzzle.position, Quaternion.identity);
            // attackEffect.Play();
        }
    }

    //Codigo temporario apenas para o trailer
     // ServerRpc to request the action from the server
    [ServerRpc(RequireOwnership = false)]
    void RequestSetActiveServerRpc(bool isActive, string target)
    {
        switch (target)
        {
            case "smallDood":
                smallDood.SetActive(isActive);
                UpdateClientRpc(isActive, "smallDood");
                break;
            case "bigDood":
                bigDood.SetActive(isActive);
                UpdateClientRpc(isActive, "bigDood");
                break;
            case "jumpScare":
                jumpScare.SetActive(isActive);
                UpdateClientRpc(isActive, "jumpScare");
                break;
        }
    }

    // ClientRpc to inform all clients to set the object active/inactive
    [ClientRpc]
    void UpdateClientRpc(bool isActive, string target)
    {
        switch (target)
        {
            case "smallDood":
                smallDood.SetActive(isActive);
                break;
            case "bigDood":
                bigDood.SetActive(isActive);
                break;
            case "jumpScare":
                jumpScare.SetActive(isActive);
                break;
        }
    }
}
