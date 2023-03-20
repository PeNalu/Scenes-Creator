using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    [SerializeField]
    private GameObject tileObject;

    [SerializeField]
    private GameObject tileContent;

    public float H;
    public float G;

    //Stored required properties.
    private TileMap tileMap;
    private Vector2Int pos;
    private TileType tileType = TileType.Air;
    private Tile connect;

    public void Initialize(TileMap tileMap, Vector2Int pos)
    {
        this.tileMap = tileMap;
        this.pos = pos;
    }

    #region [Getter / Setter]
    public bool HasNeighborsType(TileType type)
    {
        return tileMap.HasNeighborsType(pos, type);
    }

    public void SetTileType(TileType tileType)
    {
        this.tileType = tileType;
    }

    public TileType GetTileType()
    {
        return tileType;
    }

    public bool HasEmptySide()
    {
        return GetNeighbors().Count == 4;
    }

    public List<Tile> GetNeighborsByType(TileType type)
    {
        return tileMap.GetNeighborsByType(pos, type);
    }

    public List<Tile> GetEmptyNeighbors()
    {
        return tileMap.GetEmptyNeighbors(pos);
    }

    public List<Tile> GetNeighbors()
    {
        return tileMap.GetNeighbors(pos);
    }

    public void SetTileObject(GameObject gameObject)
    {
        Vector3 objPos = new Vector3(pos.x, tileMap.transform.position.y, pos.y);
        tileObject = gameObject;
        tileObject.transform.position = objPos;
    }

    public void ChangeTileObject(GameObject gameObject)
    {
        GameObject.Destroy(tileObject);
        SetTileObject(gameObject);
    }

    public GameObject GetTileObject()
    {
        return tileObject;
    }

    public Vector2Int GetPosition()
    {
        return pos;
    }

    public TileMap GetTileMap()
    {
        return tileMap;
    }

    public void SetTileMap(TileMap tileMap)
    {
        this.tileMap = tileMap;
    }

    public void SetTileContent(GameObject gameObject)
    {
        tileContent = gameObject;
    }

    public GameObject GetTileContent()
    {
        return tileContent;
    }

    public Tile GetConnect()
    {
        return connect;
    }

    public void SetConnect(Tile tile)
    {
        connect = tile;
    }

    public float GetDistance(Vector2Int vector)
    {
        return Vector2Int.Distance(pos, vector);
    }
    #endregion
}
