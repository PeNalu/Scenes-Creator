using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    //Stored required components.
    private Transform playertransform;

    private void Awake()
    {
        playertransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Teleport()
    {
        Vector3 point1 = transform.position + transform.forward;
        Vector3 point2 = transform.position - transform.forward;

        float lenght1 = Vector3.Distance(point1, playertransform.position);
        float lenght2 = Vector3.Distance(point2, playertransform.position);

        playertransform.position = lenght1 >= lenght2 ? point1 : point2;
    }

    private void OnDrawGizmos()
    {
        Vector3 point1 = transform.position + transform.forward;
        Vector3 point2 = transform.position - transform.forward;

        Gizmos.DrawSphere(point1, 0.1f);
        Gizmos.DrawSphere(point2, 0.1f);
    }
}
