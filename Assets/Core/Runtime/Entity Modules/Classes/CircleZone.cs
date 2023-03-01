using UnityEditor;
using UnityEngine;
using Voxell.Inspector;

public class CircleZone : Zone
{
    [SerializeField]
    private float radius;

    [SerializeField]
    private float offset = 0;

    //Stored required properties.

    [Button]
    public override Vector3 CalculateRandomPoint()
    {
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        float rad = Random.Range(offset, radius);

        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);

        return transform.position + Vector3.ProjectOnPlane(new Vector3(x, 0, y) * rad, transform.up);
    }

#if UNITY_EDITOR
    protected override void OnVisualize()
    {
        Handles.color = Color.green;
        Handles.DrawWireArc(transform.position, transform.up, -transform.right, 360, radius, 1f);

        Handles.color = Color.red;
        Handles.DrawWireArc(transform.position, transform.up, -transform.right, 360, offset, 1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(CalculateRandomPoint(), 0.1f);
    }
#endif
}
