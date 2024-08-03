using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    [SerializeField] private Rigidbody[] rigidbodys;
    [SerializeField] private int solverIterations;
    [SerializeField] private int velSolverIterations;
    [SerializeField] private int maxAngularVelocity;

    private void Start()
    {
        if(rigidbodys != null)
        {
            foreach(Rigidbody rigidbody in rigidbodys)
            {
                rigidbody.solverIterations = solverIterations;
                rigidbody.solverVelocityIterations = velSolverIterations;
                rigidbody.maxAngularVelocity = maxAngularVelocity;
            }
        }
    }
}
