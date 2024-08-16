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

    public override void OnStartLocalPlayer()
    {
        jumpInput = actionAsset.FindAction("Jump");
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
