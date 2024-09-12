using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    public float playerSpeed = 5.0f;
    private bool isGrounded;
    public float gravity = -24f;
    public float jumpHeight = 1.5f;

    private bool crouching = false;
    private float crouchTimer = 1;
    private bool lerpCrouch = false;
    private bool sprinting = false;
    [SerializeField] private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();   
        Cursor.lockState = CursorLockMode.Locked;

        // Get the Animator component
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if(!IsOwner) return;
        
        isGrounded = controller.isGrounded;

        if(lerpCrouch) 
        {
            crouchTimer += Time.deltaTime;
            float p  = crouchTimer / 1f;
            p *= p;
            //change this to move the camera and not the entire player
            if(crouching)
                controller.height = Mathf.Lerp(controller.height, 1, p);
            else 
                controller.height = Mathf.Lerp(controller.height, 2, p);

            
            if (p > 1) 
            {
                lerpCrouch = true; 
                crouchTimer = 0f;
            }
        }

    }

    //Receive the inputs from or InputManager.cs and apply them to our character controller.
    public void ProcessMovement(Vector2 input) 
    {
        //MoveDirection
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * playerSpeed * Time.deltaTime);

        //Gravity
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);

        // Check if player is moving and update animator
        bool isWalking = moveDirection.x != 0 || moveDirection.z != 0;
        animator.SetBool("IsWalking", isWalking);

    }

    public void Jump() 
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
        } 
    }

    public void Crouch() 
    {
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
        if(crouching) 
            playerSpeed = 2.5f;
        else 
            playerSpeed = 5f;   
    }

    public void Sprint () 
    {
        sprinting = !sprinting;
        if(sprinting && !crouching) 
            playerSpeed = 10f;
        else 
            playerSpeed = 5f;   
    }
}
