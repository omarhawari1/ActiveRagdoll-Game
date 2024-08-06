using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActiveRagdollController : NetworkBehaviour
{
    [SerializeField] private bool updateValuesOnUpdate = false;
    public ConfigJoint head;
    public ConfigJoint torso;
    public ConfigJoint[] arms;
    public ConfigJoint[] forearms;
    public ConfigJoint[] legs;
    public ConfigJoint[] knees;

    private List<ConfigJoint> joints;

    [SerializeField] private Rigidbody[] rigidbodies;
    [SerializeField] private int solverIterations;
    [SerializeField] private int velSolverIterations;
    [SerializeField] private int maxAngularVelocity;

    public override void OnStartLocalPlayer()
    {
        joints = new List<ConfigJoint>() { head, torso, arms[0], arms[1], forearms[0], forearms[1], legs[0], legs[1], knees[0], knees[1]};
        foreach (ConfigJoint joint in joints)
        {
            if (joint != null)
            {
                joint.UpdateJointVariables();
            }
        }
        InitializeRigidboides();
        foreach(ConfigJoint joint in joints)
        {
            joint.Initialize();
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) { return; }
        if (updateValuesOnUpdate)
        {
            foreach (ConfigJoint joint in joints)
            {
                if (joint != null)
                {
                    joint.UpdateJointVariables();
                }
            }
        }
        SyncRagdollAndAnimation();
    }

    private void SyncRagdollAndAnimation()
    {
        foreach(ConfigJoint joint in joints)
        {
            joint.SyncAnimation();
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
public class ConfigJoint
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
