using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Zone : MonoBehaviour
{
    [SerializeField]
    private bool debug = false;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (debug)
        {
            OnVisualize();
        }
    }
#endif

    public abstract Vector3 CalculateRandomPoint();
    protected abstract void OnVisualize();
}
