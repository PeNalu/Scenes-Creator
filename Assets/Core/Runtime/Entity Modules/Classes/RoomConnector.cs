using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConnector : MonoBehaviour
{
    //Stored required components.
    [SerializeField]
    private Transform child;
    [SerializeField]
    private Transform connectionObj;
    private RoomConnectorData connectorData;

    #region [Getter / Setter]
    public void SetConnectorData(RoomConnectorData data)
    {
        connectorData = data;
    }

    public RoomConnectorData GetConnectorData()
    {
        return connectorData;
    }

    public void SetConnectionObject(Transform transform)
    {
        connectionObj = transform;
    }

    public void SetConnection(Transform child)
    {
        this.child = child;
    }

    public void Break—onnection()
    {
        child = null;
    }

    public Transform GetConnectionObject()
    {
        return connectionObj;
    }

    public Transform GetChild()
    {
        return child;
    }

    public bool HasConnection()
    {
        return child != null;
    }
    #endregion
}
