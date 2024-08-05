using Cinemachine;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveForce;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Animator rigAnimator;
    [SerializeField] private Transform playerHead;
    private Vector2 movementInput;
    private Rigidbody rb;
    private Camera playerCam;

    private float currentRotationVelocity;

    private CinemachineFreeLook thirdPersonCameraController;

    public override void OnStartLocalPlayer()
    {
        thirdPersonCameraController = FindAnyObjectByType<CinemachineFreeLook>();
        thirdPersonCameraController.Follow = transform;
        thirdPersonCameraController.LookAt = playerHead;

        rb = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!isLocalPlayer) { return; }
        setAnimationVelocity();
    }

    private void setAnimationVelocity()
    {
        float velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        rigAnimator.SetFloat("Velocity", velocity);
        rigAnimator.SetFloat("Input", movementInput.magnitude);
    }

    private void FixedUpdate()
    {
        if(!isLocalPlayer) { return; }
        MovePlayer();
        RotatePlayer();
    }
    private void RotatePlayer()
    {
        Vector3 direction = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCam.transform.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentRotationVelocity, rotationSpeed * 0.1f);

            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    private void MovePlayer()
    {
        rb.velocity = transform.forward * moveForce * movementInput.magnitude;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }
}
