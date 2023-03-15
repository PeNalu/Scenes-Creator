using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Entity : TileObject
{
    [SerializeField]
    private int degugIndex = 0;

    [SerializeField]
    private List<EntityZone> entityZones;

    [SerializeField]
    private List<EntityZone2> entityZone2s;

    [SerializeField]
    private bool staticEntity = false;

    //Stored required properties.
    private TextEntity textEntity;

    public void CalculateZones(TileMap tileMap)
    {
        string name = gameObject.name;
        entityZone2s = new List<EntityZone2>();

        EntityZone2 dZone = new EntityZone2();
        dZone.position = "default";
        EntityZone2 onZone = new EntityZone2();
        onZone.position = "on";
        List<Vector2Int> onTiles = new List<Vector2Int>();
        foreach (Vector2Int tilePos in GetTiles())
        {
            Tile currentTile = tileMap.GetTile(tilePos);
            if(currentTile.GetTileType() == TileType.Wall || currentTile.GetTileType() == TileType.Floor)
            {
                List<Tile> nb = tileMap.GetNeighborsByType(tilePos, TileType.Floor);
                foreach (Tile item in nb)
                {
                    onTiles.Add(tilePos);
                }
            }
            else
            {
                if (!tileMap.HasNeighborsType(tilePos, TileType.Wall))
                {
                    onTiles.Add(tilePos);
                }
            }
        }
        onZone.positions = onTiles;
        dZone.positions = onTiles;

        EntityZone2 nextZone = new EntityZone2();
        nextZone.position = "next";
        EntityZone2 aZone = new EntityZone2();
        aZone.position = "against";
        EntityZone2 nZone = new EntityZone2();
        nZone.position = "neer";
        List<Vector2Int> nTiles = new List<Vector2Int>();
        foreach (Vector2Int tilePos in GetTiles())
        {
            List<Tile> nb = tileMap.GetNeighborsByType(tilePos, TileType.Floor);
            foreach (Tile item in nb)
            {
                if (!GetTiles().Contains(item.GetPosition()))
                {
                    nTiles.Add(item.GetPosition());
                }
            }
        }
        aZone.positions = nTiles;
        nZone.positions = nTiles;
        nextZone.positions = nTiles;

        entityZone2s.Add(aZone);
        entityZone2s.Add(dZone);
        entityZone2s.Add(onZone);
        entityZone2s.Add(nZone);
        entityZone2s.Add(nextZone);
    }

    public EntityZone2 GetZoneByTag(string tag)
    {
        EntityZone2 entityZone = entityZone2s.Where(x => x.position == tag).FirstOrDefault();
        if (entityZone == null)
        {
            entityZone = entityZone2s.Where(x => x.position == "default").FirstOrDefault();
        }

        return entityZone;
    }

    public Vector3 GetPosition(string tag)
    {
        EntityZone entityZone = entityZones.Where(x => x.position == tag).FirstOrDefault();
        if (entityZone == null)
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

    private void OnDrawGizmosSelected()
    {
        if (entityZone2s != null && entityZone2s.Count == 5)
        {
            Handles.color = Color.red;
            foreach (Vector2Int item in entityZone2s[degugIndex].positions)
            {
                Handles.DrawSolidDisc(new Vector3(item.x, 0, item.y),Vector3.up, 0.1f);
            }
        }

        Handles.color = Color.black;
        if (GetTiles() != null)
        {
            foreach (Vector2Int item in GetTiles())
            {
                Vector3 pos = new Vector3(item.x, 0, item.y);
                Handles.DrawSolidDisc(pos, Vector3.up, 0.1f);
            }
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

/*        Gizmos.color = Color.black;
        foreach (Vector2Int item in GetTiles())
        {
            Vector3 pos = new Vector3(item.x, 0 ,item.y);
            Gizmos.DrawSphere(pos, 0.1f);
        }*/
    }
}

[System.Serializable]
public class EntityZone2
{
    public string position;
    public List<Vector2Int> positions;

    public Vector2Int GetRandomPosition(TileMap tileMap, Vector2Int size)
    {
        List<Vector2Int> validTiles = GetValidTiles(tileMap, size);

        if (validTiles.Count == 0) return Vector2Int.zero;
        return validTiles[Random.Range(0, validTiles.Count)];
    }

    public bool HasSpace(TileMap tileMap, Vector2Int size)
    {
        List<Vector2Int> validTiles = GetValidTiles(tileMap, size);
        return validTiles.Count > 0;
    }

    public List<Vector2Int> GetValidTiles(TileMap tileMap, Vector2Int size)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (Vector2Int tile in positions)
        {
            if(HasObstacle(tileMap, size, tile))
            {
                continue;
            }
            result.Add(tile);
        }

        return result;
    }

    private bool HasObstacle(TileMap tileMap, Vector2Int size, Vector2Int tile)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int newPos = new Vector2Int(tile.x + x, tile.y + y);
                if (!tileMap.TryGetTile(newPos, out Tile t) || t.GetTileType() != TileType.Floor)
                {
                    return true;
                }
            }
        }

        return false;
    }
}

[System.Serializable]
public class EntityZone
{
    public string position;
    public Zone zone;
}
