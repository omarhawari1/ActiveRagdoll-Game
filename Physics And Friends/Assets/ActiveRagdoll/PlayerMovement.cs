using Cinemachine;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private InputActionAsset playerInputActionAsset;
    [SerializeField] private float moveForce;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Animator rigAnimator;
    [SerializeField] private Transform playerHead;
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float jumpForce;
    [SerializeField] private float airMultiplier;


    private Vector2 movementInput;
    private Vector2 lookInput;
    private Rigidbody rb;
    private Camera playerCam;

    private bool isGrounded;
    private bool readyToJump = true;

    private InputAction jumpInput;


    private CinemachineFreeLook thirdPersonCameraController;

    public override void OnStartLocalPlayer()
    {
        thirdPersonCameraController = FindAnyObjectByType<CinemachineFreeLook>();
        thirdPersonCameraController.Follow = transform;
        thirdPersonCameraController.LookAt = playerHead;

        rb = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;

        jumpInput = playerInputActionAsset.FindAction("Jump");
    }

    private void Update()
    {
        if (!isLocalPlayer) { return; }
        SetAnimationVelocity();
        CheckGrounded();
        if(jumpInput.IsPressed() && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
        if(isGrounded) { rb.drag = groundDrag; }
        else { rb.drag = 0f; }
    }

    private void SetAnimationVelocity()
    {
        if (isGrounded)
        {
            float velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
            rigAnimator.SetFloat("Velocity", velocity);
            rigAnimator.SetFloat("Input", movementInput.magnitude);
        }
        else
        {
            rigAnimator.SetFloat("Velocity", 0f);
            rigAnimator.SetFloat("Input", 0f);
        }
    }

    private void FixedUpdate()
    {
        if(!isLocalPlayer) { return; }
        MovePlayer();
        RotatePlayer();
    }
    private void RotatePlayer()
    {
        if (movementInput.sqrMagnitude > 0.1f) // Ensure the input is significant enough
        {
            Vector3 cameraForward = playerCam.transform.forward;
            Vector3 cameraRight = playerCam.transform.right;

            // We only care about the horizontal direction
            cameraForward.y = 0;
            cameraRight.y = 0;

            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 targetDirection = cameraForward * movementInput.y + cameraRight * movementInput.x;

            if (targetDirection == Vector3.zero)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    private void MovePlayer()
    {
        if (isGrounded)
        {
            rb.velocity = transform.forward * moveForce * movementInput.magnitude;
        }
        else
        {
            rb.velocity = transform.forward * moveForce * movementInput.magnitude * airMultiplier;
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }
}
