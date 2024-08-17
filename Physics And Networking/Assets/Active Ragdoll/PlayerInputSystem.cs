using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : NetworkBehaviour
{
    //showen in inspector:
    public InputActionAsset actionAsset = null;

    //hidden from inspector:
    [HideInInspector] public Vector2 movementInput = Vector2.zero;
    [HideInInspector] public InputAction jumpInput = null;
    [HideInInspector] public InputAction leftHandGrab = null;
    [HideInInspector] public InputAction rightHandGrab = null;
    [HideInInspector] public InputAction leftHandPunch = null;
    [HideInInspector] public InputAction rightHandPunch = null;

    public override void OnStartLocalPlayer()
    {
        jumpInput = actionAsset.FindAction("Jump");
        leftHandGrab = actionAsset.FindAction("Left Hand Grab");
        rightHandGrab = actionAsset.FindAction("Right Hand Grab");
        leftHandPunch = actionAsset.FindAction("Left Hand Punch");
        rightHandPunch = actionAsset.FindAction("Right Hand Punch");
    }
    private void FixedUpdate()
    {
        if(!isLocalPlayer) { return; }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }
}
