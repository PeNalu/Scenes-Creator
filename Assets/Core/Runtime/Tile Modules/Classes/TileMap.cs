using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    //Stored required properties.
    [SerializeField]
    private Dictionary<Vector2Int, Tile> tileMap;

    private void Awake()
    {
        tileMap = new Dictionary<Vector2Int, Tile>();
    }

    public void Initialize()
    {
        tileMap = new Dictionary<Vector2Int, Tile>();
    }

    public void CreateTile(Vector2Int pos, Tile tile)
    {
        tileMap.Add(pos, tile);
        tile.Initialize(this, pos);
    }

    private void OnDrawGizmos()
    {
/*        Gizmos.color = Color.blue;

        foreach (KeyValuePair<Vector2Int, Tile> item in tileMap)
        {
            if (item.Value.GetTileType() == TileType.Floor)
            {
                Gizmos.DrawSphere(new Vector3(item.Key.x, transform.position.y, item.Key.y), 0.1f);
            }
        }*/
    }

    #region [Getter / Setter]
    public Tile GetTile(Vector2Int pos)
    {
        return tileMap[pos];
    }

    public bool HasNeighborsType(Vector2Int pos, TileType type)
    {
        List<Tile> tiles = GetNeighborsByType(pos, type);
        return tiles.Count != 0;
    }

    public List<Tile> GetNeighborsByType(Vector2Int pos, TileType type)
    {
        List<Tile> neighbors = GetNeighbors(pos);
        List<Tile> result = new List<Tile>();

        foreach (Tile tile in neighbors)
        {
            if (tile.GetTileType() == type)
            {
                result.Add(tile);
            }
        }

        return result;
    }

    public List<Tile> GetEmptyNeighbors(Vector2Int pos)
    {
        List<Tile> neighbors = GetNeighbors(pos);
        List<Tile> result = new List<Tile>();

        foreach (Tile tile in neighbors)
        {
            if (tile.GetTileObject() == null)
            {
                result.Add(tile);
            }
        }

        return result;
    }

    public List<Tile> GetNeighbors(Vector2Int pos)
    {
        List<Tile> neighbors = new List<Tile>();
        List<Vector2Int> vectors = new List<Vector2Int>()
        {
        new Vector2Int(pos.x, pos.y + 1),
        new Vector2Int(pos.x, pos.y - 1),
        new Vector2Int(pos.x - 1, pos.y),
        new Vector2Int(pos.x + 1, pos.y)
        };

        foreach (Vector2Int item in vectors)
        {
            if (TryGetTile(item, out Tile tile))
            {
                neighbors.Add(tile);
            }
            else
            {
                Tile newTile = new Tile();
                CreateTile(item, newTile);
                neighbors.Add(newTile);
            }
        }

        return neighbors;
    }

    public bool TryGetTile(Vector2Int pos, out Tile tile)
    {
        if (tileMap.ContainsKey(pos))
        {
            tile = tileMap[pos];
            return true;
        }

        tile = null;
        return false;
    }
    #endregion
}
