using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdollSystem : NetworkBehaviour
{
    //showen in inspector:
    [SerializeField] private bool updateValuesOnUpdate = false;
    public JointController head = null;
    public JointController torso = null;
    public JointController[] arms = null;
    public JointController[] forearms = null;
    public JointController[] legs = null;
    public JointController[] knees = null;
    [SerializeField] private Rigidbody[] rigidbodies = null;
    [SerializeField] private RigidbodyInterpolation interpolatorType = RigidbodyInterpolation.Interpolate;
    [SerializeField] private int solverIterations = 8;
    [SerializeField] private int velSolverIterations = 8;
    [SerializeField] private int maxAngularVelocity = 20;

    //private:
    private List<JointController> jointControllers = null;

    private void Start()
    {
        CmdStart();
    }
    [Command]
    private void CmdStart()
    {
        jointControllers = new List<JointController>() { head, torso, arms[0], arms[1], forearms[0], forearms[1], legs[0], legs[1], knees[0], knees[1] };
        foreach (JointController joint in jointControllers)
        {
            if (joint != null)
            {
                joint.Initialize();
            }
        }
        InitializeRigidboides();
    }

    private void Update()
    {
        if (!isLocalPlayer) { return; }
        CmdSyncRagdollAndAnimation();
    }
    [Command]
    private void CmdSyncRagdollAndAnimation()
    {
        foreach (JointController jointController in jointControllers)
        {
            jointController.SyncAnimation();
        }
    }
    private void InitializeRigidboides()
    {
        if (rigidbodies != null)
        {
            foreach (Rigidbody rigidbody in rigidbodies)
            {
                rigidbody.solverIterations = solverIterations;
                rigidbody.solverVelocityIterations = velSolverIterations;
                rigidbody.maxAngularVelocity = maxAngularVelocity;
                rigidbody.interpolation = interpolatorType;
            }
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                Collider colliderA = rigidbodies[i].GetComponent<Collider>();

                for (int j = i + 1; j < rigidbodies.Length; j++)
                {
                    Collider colliderB = rigidbodies[j].GetComponent<Collider>();
                    Physics.IgnoreCollision(colliderA, colliderB);
                }
            }
        }
    }
}

[Serializable]
public class JointController
{
    public JointType jointType;
    public ConfigurableJoint joint;
    public Transform correspondingAnimatedJoint;
    public float rotationSpring;
    public float rotationDamping;
    public float rotationMaxForce;
    public float mass;
    public float Strength = 1f;

    private Quaternion jointInitialRotation;

    public void Initialize()
    {
        if (joint == null) { return; }
        UpdateJointVariables();
        jointInitialRotation = joint.transform.localRotation;
    }
    public void UpdateJointVariables()
    {
        if (joint == null) { return; }
        JointDrive newDrive = new JointDrive();
        newDrive.positionSpring = rotationSpring * Strength;
        newDrive.positionDamper = rotationDamping;
        newDrive.maximumForce = rotationMaxForce * Strength;
        joint.slerpDrive = newDrive;
        Rigidbody rb = joint.GetComponent<Rigidbody>();
        rb.mass = mass;
    }

    public void SyncAnimation()
    {
        if (joint == null) { return; }
        ConfigurableJointExtensions.SetTargetRotationLocal(joint, correspondingAnimatedJoint.localRotation, jointInitialRotation);
    }

    public void SetStrength(float value)
    {
        Strength = value;
        UpdateJointVariables();
    }
}

public enum JointType
{
    None = 0,
    Left,
    Right
}
