using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    [SerializeField] ActiveRagdollGrab activeRagdollGrab;
    [SerializeField] ConfigurableJoint armJoint;

    private ConfigurableJoint joint;
    private Rigidbody grabbedObject;

    public void Grab()
    {
        if (grabbedObject == null)
        {
            TryGrabObject();
        }
    }

    private void TryGrabObject()
    {
        Collider[] collisions = Physics.OverlapSphere(transform.position, activeRagdollGrab.grabRadius);
        List<Rigidbody> objects = new List<Rigidbody> ();
        foreach(Collider collision in collisions)
        {
            if(collision.TryGetComponent(out Rigidbody objectRigidbody) && collision.gameObject.layer != gameObject.layer)
            {
                objects.Add(objectRigidbody);
            }
        }
        if(objects.Count > 0)
        {
            grabbedObject = objects[0];
            joint = armJoint.gameObject.AddComponent<ConfigurableJoint>();
            joint.connectedBody = grabbedObject;
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;
        }
    }
    public void ReleaseObject()
    {
        if(joint != null)
        {
            Destroy(joint);
            grabbedObject = null;
        }
    }
}
