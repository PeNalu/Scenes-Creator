using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField]
    private List<EntityZone> entityZones;

    [SerializeField]
    private bool staticEntity = false;

    //Stored required properties.
    private TextEntity textEntity;

    public Vector3 GetPosition(string tag)
    {
        EntityZone entityZone = entityZones.Where(x => x.position == tag).FirstOrDefault();
        if(entityZone == null)
        {
            entityZone = entityZones.Where(x => x.position == "default").FirstOrDefault();
        }

        return entityZone.zone.CalculateRandomPoint();
    }

    public void SetTextEntity(TextEntity value)
    {
        textEntity = value;
    }

    public TextEntity GetTextEntity()
    {
        return textEntity;
    }

    public void IsStatic(bool value)
    {
        staticEntity = value;
    }

    public bool IsStatic()
    {
        return staticEntity;
    }
}

[System.Serializable]
public class EntityZone
{
    public string position;
    public Zone zone;
}
