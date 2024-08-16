using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;

public class PlayerMainSystem : NetworkBehaviour
{
    //showen in the inspector:
    [SerializeField] private Transform cameraLookAt = null;
    [SerializeField] private Transform playerRoot = null;
    public Animator rigAnimator = null;
    private CinemachineFreeLook thirdPersonCamController = null;


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Cursor.lockState = CursorLockMode.Locked;
        SetPlayerCamera();
    }
    private void SetPlayerCamera()
    {
        thirdPersonCamController = FindAnyObjectByType<CinemachineFreeLook>();
        thirdPersonCamController.Follow = playerRoot;
        thirdPersonCamController.LookAt = cameraLookAt;
    }

}
