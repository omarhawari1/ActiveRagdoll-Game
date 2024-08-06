using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class ActiveRagdollGrab : NetworkBehaviour
{
    [SerializeField] private float armGrabStrength;
    [SerializeField] private Arm leftArm;
    [SerializeField]private Arm rightArm;
    [SerializeField] private InputActionAsset playerInputActionAsset;
    public float grabRadius;

    private InputAction rightHandGrabInput;
    private InputAction leftHandGrabInput;

    private ActiveRagdollController activeRagdollController;

    private ConfigJoint leftArmConfigJoint;
    private ConfigJoint rightArmConfigJoint;

    public override void OnStartLocalPlayer()
    {
        activeRagdollController = GetComponent<ActiveRagdollController>();
        rightHandGrabInput = playerInputActionAsset.FindAction("Right Hand Grab");
        leftHandGrabInput = playerInputActionAsset.FindAction("Left Hand Grab");

        foreach (ConfigJoint joint in activeRagdollController.arms)
        {
            if (joint.jointType == JointType.Left)
            {
                leftArmConfigJoint = joint;
            }
            if(joint.jointType== JointType.Right)
            {
                rightArmConfigJoint = joint;
            }
        }
    }

    private void Update()
    {
        if(rightHandGrabInput.IsPressed())
        {
            if(rightArmConfigJoint!= null)
            {
                rightArmConfigJoint.SetStrength(armGrabStrength);
            }
            rightArm.Grab();
        }
        else
        {
            if (rightArmConfigJoint != null)
            {
                rightArmConfigJoint.SetStrength(1f);
            }
            rightArm.Release();
        }
        if(leftHandGrabInput.IsPressed())
        {
            if (leftArmConfigJoint != null)
            {
                leftArmConfigJoint.SetStrength(armGrabStrength);
            }
            leftArm.Grab();
        }
        else
        {
            if (leftArmConfigJoint != null)
            {
                leftArmConfigJoint.SetStrength(1f);
            }
            leftArm.Release();
        }
    }
}

[Serializable]
public class Arm
{
    public Transform IKTarget;
    public Transform armGrabPosition;
    public GrabObject HandGrab;

    public void Grab()
    {
        IKTarget.position = armGrabPosition.position;
        HandGrab.Grab();
    }
    public void Release()
    {
        HandGrab.ReleaseObject();
    }
}
