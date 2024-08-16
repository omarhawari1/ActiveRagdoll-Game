using Mirror;
using System;
using UnityEngine;

public class PlayerMovementSystem : NetworkBehaviour
{
    //showen in inspector:
    [Header("Movement Settings:")]
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float moveForce = 10f;
    [SerializeField] private float airMultiplier = 0.4f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask groundLayer = default;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float jumpCoolDown = 0.5f;
    [SerializeField] private float jumpForce = 500f;

    [SerializeField] private Animator rigAnimator;

    private PlayerInputSystem playerInputSystem = null;
    private Rigidbody rb = null;

    private bool readyToJump = true;
    private bool isGrounded = false;

    private Camera playerCam;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        playerCam = Camera.main;
    }

    private void Start()
    {
        rb = transform.parent.parent.GetComponent<Rigidbody>();
        playerInputSystem = transform.parent.GetComponentInChildren<PlayerInputSystem>();
    }
    private void Update()
    {
        if (!isLocalPlayer) { return; }
        GroundCheck();
        SetAnimationVelocity();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
        if (isGrounded) { rb.drag = groundDrag; }
        else { rb.drag = 0f; }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) { return; }
        CmdMovePlayer(playerInputSystem.movementInput);
        RotatePlayer();
        if (playerInputSystem.jumpInput.IsPressed() && readyToJump && isGrounded)
        {
            readyToJump = false;
            CmdJump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    [Command]
    private void CmdJump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    [Command]
    private void CmdMovePlayer(Vector2 movementInput)
    {
        rb.velocity = rb.transform.forward * moveForce * movementInput.magnitude;
    }
    private void RotatePlayer()
    {
        //Chat GPT code:
        if (playerInputSystem.movementInput.sqrMagnitude > 0.1f) // Ensure the input is significant enough
        {
            Vector3 cameraForward = playerCam.transform.forward;
            Vector3 cameraRight = playerCam.transform.right;

            // We only care about the horizontal direction
            cameraForward.y = 0;
            cameraRight.y = 0;

            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 targetDirection = cameraForward * playerInputSystem.movementInput.y + cameraRight * playerInputSystem.movementInput.x;

            if (targetDirection == Vector3.zero)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            CmdRotatePlayer(targetRotation);
        }
    }
    [Command]
    private void CmdRotatePlayer(Quaternion targetRotation)
    {
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    }


    private void SetAnimationVelocity()
    {
        if (isGrounded)
        {
            float velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
            CmdSetAnimationVelocity(velocity, playerInputSystem.movementInput);
        }
        else
        {
            CmdSetAnimationVelocity(0f, Vector2.zero);
        }
    }
    [Command]
    private void CmdSetAnimationVelocity(float velocity, Vector2 moveInput)
    {
        rigAnimator.SetFloat("Velocity", velocity);
        rigAnimator.SetFloat("Input", moveInput.magnitude);
    }
}
