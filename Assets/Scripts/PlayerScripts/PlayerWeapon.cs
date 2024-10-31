using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using PolygonArsenal; // Import the namespace of the beam

public class PlayerWeapon : NetworkBehaviour
{
    [SerializeField] private GameObject weapon;  // Reference to the weapon GameObject
    private InputManager inputManager;
    private bool isWeaponEquipped = true;  // Track if weapon is in hand
    [SerializeField] private GameObject bigDood; // Reference to the big Enemy GameObject
    [SerializeField] private GameObject jumpScare; // Reference to the jumpscare Enemy GameObject
    [SerializeField] private GameObject smallDood; // Reference to the small Enemy GameObject
    [SerializeField] private GameObject getUp;
    [SerializeField] private GameObject enemyAttackYou;
    [SerializeField] private Animator animator;

    // Reference to the particle system
    [SerializeField] private PolygonBeamStatic beamScript; // Reference to the beam script
    
    [SerializeField] private GameObject beamPrefab; // Prefab for the beam
    private bool isAttacking = false; // Track if attack is happening


    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        
        if (weapon == null)
        {
            weapon = transform.Find("PlayerWeapon").gameObject;
        }
        if (smallDood == null)
        {
            smallDood = GameObject.Find("Enemy");
        }
        if (bigDood == null)
        {
            bigDood = GameObject.Find("BigDood");
        }
        if (jumpScare == null)
        {
            jumpScare = GameObject.Find("JumpScare");
        }
        if (enemyAttackYou == null)
        {
            enemyAttackYou = GameObject.Find("EnemyAttackYou");
        }
        if (getUp == null)
        {
            getUp = GameObject.Find("GetUp");
        }

        animator = GetComponentInChildren<Animator>();

    }

    private void Awake() 
    {
        if (beamScript == null)
        {
            beamScript = FindObjectOfType<PolygonBeamStatic>();
        }
        if (beamPrefab == null)
        {
            beamPrefab = GameObject.Find("PolyBeamStaticBlue");
        }
    }

    private void Update()
    {
        if (!IsOwner) return;  // Ensure only the owning player controls the weapon

        SwitchWeapon();
        WeaponAttack();

        // Temporary code for the trailer
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
        if (Input.GetKeyDown(KeyCode.Alpha6) && IsOwner)
        {
            RequestSetActiveServerRpc(true, "GetUp");
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) && IsOwner)
        {
            RequestSetActiveServerRpc(false, "GetUp");
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) && IsOwner)
        {
            RequestSetActiveServerRpc(false, "enemyAttackYou");
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) && IsOwner)
        {
            RequestSetActiveServerRpc(true, "enemyAttackYou");
        }
        if (Input.GetMouseButton(1)) 
        {
            animator.SetBool("IsAiming", true);
        }
        else
        {
            animator.SetBool("IsAiming", false);
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
            }
            else
            {
                // Put weapon in the hand
                Debug.Log("Weapon equipped");
                isWeaponEquipped = true;
                weapon.SetActive(true);
            }
        }
    }

    private void WeaponAttack()
    {
        if(!IsOwner) return;

        // Start the beam when the attack button is pressed
        if (isWeaponEquipped && inputManager.onFoot.WeaponAttack.ReadValue<float>() > 0)
        {
            if (!isAttacking) 
            {
                NetworkLog.LogInfoServer("Weapon attack started");
                RequestStartAttackServerRpc(OwnerClientId); // Notify the server to start the attack for all clients
                isAttacking = true;  // Set attacking state to true
            }
        }
        else
        {
            // Stop the beam when the attack button is released
            if (isAttacking)
            {
                NetworkLog.LogInfoServer("Weapon attack stopped");
                RequestStopAttackServerRpc(OwnerClientId); // Notify the server to stop the attack for all clients
                isAttacking = false;  // Reset attacking state to false
            }
        }
    }

    // Notify the server to start the attack, passing the Client ID of the player who is attacking
    [ServerRpc]
    private void RequestStartAttackServerRpc(ulong clientId)
    {
        NetworkLog.LogInfoServer("Request to start attack received");
        StartAttackClientRpc(clientId); // Notify all clients to start the attack for the specified player
    }

    // Notify the server to stop the attack, passing the Client ID of the player who is stopping
    [ServerRpc]
    private void RequestStopAttackServerRpc(ulong clientId)
    {
        NetworkLog.LogInfoServer("Request to stop attack received");
        StopAttackClientRpc(clientId); // Notify all clients to stop the attack for the specified player
    }

    // ClientRpc to notify all clients to start the beam attack from the correct player
    [ClientRpc]
    private void StartAttackClientRpc(ulong clientId)
    {
        NetworkLog.LogInfoServer("Start attack client RPC received");
        // Ensure we are enabling the beam for the correct player
        if (OwnerClientId == clientId)
        {
            beamScript.EnableBeam(); // Enable the beam and particles for the firing player
        }
    }

    // ClientRpc to notify all clients to stop the beam attack for the correct player
    [ClientRpc]
    private void StopAttackClientRpc(ulong clientId)
    {
        NetworkLog.LogInfoServer("Stop attack client RPC received");
        // Ensure we are disabling the beam for the correct player
        if (OwnerClientId == clientId)
        {
            beamScript.DisableBeam(); // Disable the beam and particles for the firing player
        }
    }

    // Temporary code for the trailer
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
            case "GetUp":
                getUp.SetActive(isActive);
                UpdateClientRpc(isActive, "GetUp");
                break;
            case "enemyAttackYou":
                enemyAttackYou.SetActive(isActive);
                UpdateClientRpc(isActive, "enemyAttackYou");
                break;
        }
    }

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
            case "GetUp":
                getUp.SetActive(isActive);
                break;
            case "enemyAttackYou":
                enemyAttackYou.SetActive(isActive);
                break;
        }
    }
}
