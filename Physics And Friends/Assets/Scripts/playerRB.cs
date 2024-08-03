using Cinemachine;
using Mirror;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class playerRB : NetworkBehaviour
{
    [Header("Player Settings:")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float playerHeight;
    [SerializeField] private float rbGroundDrag;
    public Animator animator;
    [SerializeField] private float rollForce;
    [SerializeField] private float rollCoolDown;

    [Header("World Settings:")]
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private Transform playerHead;

    [Header("Inputs:")]
    public InputActionAsset playerInputActionAsset;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector2 movementInput;

    private bool isRolling;

    private int moveSpeedHash;
    private int yVelHash;

    private Vector3 moveDirection;

    private float playerSpeed;

    private InputAction sprintInputAction;
    private InputAction RollInputAction;

    private CinemachineFreeLook thirdPersonCameraController;

    [HideInInspector]
    public Camera playerCam;

    public override void OnStartLocalPlayer()
    {
        thirdPersonCameraController = FindAnyObjectByType<CinemachineFreeLook>();
        thirdPersonCameraController.Follow = transform;
        thirdPersonCameraController.LookAt = playerHead;

        playerCam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();

        moveSpeedHash = Animator.StringToHash("moveSpeed");
        yVelHash = Animator.StringToHash("yVel");

        sprintInputAction = playerInputActionAsset.FindAction("Sprint");
        RollInputAction = playerInputActionAsset.FindAction("Jump");
    }
    private void Update()
    {
        if (!isLocalPlayer) { return; }
        groundCheck();
        handlePlayerSpeed();
        if (!isGrounded) animator.SetFloat(yVelHash, rb.velocity.y);
    }

    private void handlePlayerSpeed()
    {
        playerSpeed = sprintInputAction.IsPressed() ? sprintSpeed : walkSpeed;
    }

    private void groundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, GroundMask);

        if (isGrounded) rb.drag = rbGroundDrag;
        else { rb.drag = 0; }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) { return; }
        Move();
        Vector3 playerVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if(RollInputAction.IsPressed() && isGrounded && !isRolling && playerVel.magnitude > walkSpeed-1)
        {
            StartCoroutine(preformRoll());
        }
    }

    private IEnumerator preformRoll()
    {
        isRolling = true;
        animator.Play("Roll Motion");

        Vector3 rollDir = rb.velocity.normalized;
        float rollInitialSpeed = playerSpeed;
        rb.velocity = rollDir * rollForce * rollInitialSpeed;

        float timer = 0f;
        while(timer < animator.GetCurrentAnimatorStateInfo(0).length)
        {
            rb.velocity = rollDir * rollForce * rollInitialSpeed;
            timer += Time.deltaTime;
            yield return null;
        }
        isRolling = false;

        yield return new WaitForSeconds(rollCoolDown);
    }

    private void Move()
    {
        if(isRolling) { return; }
        Vector3 dir = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        if (dir.magnitude >= 0.1f && isGrounded)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + playerCam.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

            moveDirection = targetRotation * Vector3.forward;
            rb.AddForce(moveDirection.normalized * playerSpeed * 10f, ForceMode.Force);
        }
        Vector3 playerVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        animator.SetFloat(moveSpeedHash, playerVel.magnitude);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!isLocalPlayer) return;
        movementInput = ctx.ReadValue<Vector2>();
    }
}
