using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdollController : MonoBehaviour
{
    [SerializeField] private ConfigJoint[] joints;
    [SerializeField] private Rigidbody[] rigidbodies;
    [SerializeField] private int solverIterations;
    [SerializeField] private int velSolverIterations;
    [SerializeField] private int maxAngularVelocity;

    private void OnValidate()
    {
        foreach (ConfigJoint joint in joints)
        {
            if (joint != null)
            {
                joint.UpdateJointVariables();
            }
        }
    }
    private void Start()
    {
        InitializeRigidboides();
        foreach(ConfigJoint joint in joints)
        {
            joint.Initialize();
        }
    }
    private void Update()
    {
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
        }
    }
}

[Serializable]
public class ConfigJoint
{
    public jointGroupType jointGroupType;
    public ConfigurableJoint[] jointGroup;
    public Transform[] correspondingAnimatedJoint;
    public JointDrive jointDrive;
    public float rotationSpring;
    public float rotationDamping;
    public float rotationMaxForce;
    public float mass;

    private Quaternion[] jointInitialRotation;

    public void Initialize()
    {
        if (jointGroup == null) { return; }
        jointInitialRotation = new Quaternion[jointGroup.Length];
        for (int i = 0; i < jointGroup.Length; i++)
        {
            if (jointGroup[i] != null)
            {
                jointInitialRotation[i] = jointGroup[i].transform.rotation;
            }
        }
    }
    public void UpdateJointVariables()
    {
        if (jointGroup == null) { return; }
        foreach (ConfigurableJoint joint in jointGroup)
        {
            JointDrive newDrive = new JointDrive();
            newDrive.positionSpring = rotationSpring;
            newDrive.positionDamper = rotationDamping;
            newDrive.maximumForce = rotationMaxForce;
            joint.slerpDrive = newDrive;
            Rigidbody rb = joint.GetComponent<Rigidbody>();
            rb.mass = mass;
        }
    }

    public void SyncAnimation()
    {
        if (jointGroup == null) { return; }
        for (int i = 0; i < jointGroup.Length; i++)
        {
            ConfigurableJointExtensions.SetTargetRotationLocal(jointGroup[i], correspondingAnimatedJoint[i].rotation, jointInitialRotation[i]);
        }
    }
}

public enum jointGroupType
{
    None = 0,
    Head,
    Torso,
    Arms,
    ForeArms,
    Legs,
    Knees
}
