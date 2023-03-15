using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Voxell.Inspector;

public class Caster : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;

    private Vector3 pos;

    [Button]
    private void Cast()
    {
/*        if(Physics.Linecast(transform.position, transform.position + (Vector3.down * 10), out RaycastHit hit, 0))
        {
            pos = hit.point;
        }*/
    }

    private void OnDrawGizmos()
    {
        Vector3 hitPos = transform.position;
        Ray ray = new Ray(hitPos, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f, layerMask))
        {
            pos = hitInfo.point;
        }

        Handles.color = Color.red;
        Handles.DrawSolidDisc(pos, Vector3.up, 0.1f);

        Handles.DrawLine(transform.position, transform.position + (Vector3.down * 10));
    }
}
