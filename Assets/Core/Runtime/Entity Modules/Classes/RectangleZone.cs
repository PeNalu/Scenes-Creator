using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RectangleZone : Zone
{
    [SerializeField]
    private float width = 1f;

    [SerializeField]
    private float height = 1f;

    public override Vector3 CalculateRandomPoint()
    {
        float x = Random.Range(-(width / 2f), (width / 2f));
        float y = Random.Range(-(height / 2f), (height / 2f));

        Vector3 point = transform.TransformPoint(new Vector3(x, 0, y));
        return point;
    }

    protected override void OnVisualize()
    {
        Vector3 p1 = transform.TransformPoint(new Vector3(-(width / 2f), 0, (height / 2f)));
        Vector3 p2 = transform.TransformPoint(new Vector3((width / 2f), 0, (height / 2f)));
        Vector3 p3 = transform.TransformPoint(new Vector3(-(width / 2f), 0, -(height / 2f)));
        Vector3 p4 = transform.TransformPoint(new Vector3((width / 2f), 0, -(height / 2f)));

        Handles.DrawLine(p1, p2);
        Handles.DrawLine(p3, p4);
        Handles.DrawLine(p1, p3);
        Handles.DrawLine(p2, p4);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(p1, 0.1f);
        Gizmos.DrawSphere(p2, 0.1f);
        Gizmos.DrawSphere(p3, 0.1f);
        Gizmos.DrawSphere(p4, 0.1f);


        Gizmos.color = Color.red;
        Gizmos.DrawSphere(CalculateRandomPoint(), 0.1f);
    }
}
