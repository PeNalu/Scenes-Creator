using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour
{
    //Stored required components.
    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Grab()
    {
        rigidbody.isKinematic = true;
    }

    public void Drop()
    {
        rigidbody.isKinematic = false;
    }
}
