using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdollController : MonoBehaviour
{
    [SerializeField] private bool updateJointValues;
    [SerializeField] private ConfigJoints[] joints;
    [SerializeField] private Rigidbody[] rigidbodys;
    [SerializeField] private int solverIterations;
    [SerializeField] private int velSolverIterations;
    [SerializeField] private int maxAngularVelocity;

    private void OnValidate()
    {
        InitializeJoints();
    }
    private void Start()
    {
        InitializeRigidboides();
    }
    private void Update()
    {
        if (updateJointValues)
        {
            InitializeJoints();
        }
    }


    private void InitializeJoints()
    {
        foreach (ConfigJoints joint in joints)
        {
            if (joint != null)
            {
                joint.Init();
            }
        }
    }
    private void InitializeRigidboides()
    {
        if (rigidbodys != null)
        {
            foreach (Rigidbody rigidbody in rigidbodys)
            {
                rigidbody.solverIterations = solverIterations;
                rigidbody.solverVelocityIterations = velSolverIterations;
                rigidbody.maxAngularVelocity = maxAngularVelocity;
            }
        }
    }
}

[Serializable]
public class ConfigJoints
{
    public ConfigurableJoint[] joints;
    public JointDrive jointDrive;
    public float rotationSpring;
    public float rotationDamping;
    public float rotationMaxForce;
    public float mass;

    public void Init()
    {
        foreach (ConfigurableJoint joint in joints)
        {
            if(joint == null) { return; }
            UpdateVariables(joint);
        }
    }
    private void UpdateVariables(ConfigurableJoint joint)
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
