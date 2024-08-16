using Mirror;
using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [HideInInspector][SyncVar] public string playerId;

    private Rigidbody rb;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSetPlayerId("Player_" + netId);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    [Command]
    private void CmdSetPlayerId(string id)
    {
        playerId = id;
    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer) { return; }
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        CmdSendMovement(horizontal, vertical);
    }

    [Command]
    private void CmdSendMovement(float horizontal, float vertical)
    {
        Move(horizontal, vertical);
    }

    public void Move(float horizontal, float vertical)
    {
        if(!isServer) { return; }
        Vector3 newVelocity = new Vector3(horizontal, 0, vertical).normalized;
        rb.AddForce(newVelocity * moveSpeed, ForceMode.Force);
    }
}
