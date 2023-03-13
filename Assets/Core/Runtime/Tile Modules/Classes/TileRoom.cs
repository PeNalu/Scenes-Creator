using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRoom : MonoBehaviour
{
    [SerializeField]
    private GameObject floorTile;

    [SerializeField]
    private GameObject wallTile;

    [SerializeField]
    private GameObject doorTile;

    [SerializeField]
    [Range(1, 20)]
    private int minRoomHeight = 1;

    [SerializeField]
    [Range(1, 20)]
    private int maxRoomHeight = 5;

    [SerializeField]
    [Range(1, 20)]
    private int maxRoomWidth = 5;

    [SerializeField]
    [Range(1, 20)]
    private int mixRoomWidth = 5;

    //Stored required properties.
    private TileMap tileMap;
    private Vector2Int roomSize = Vector2Int.one;
    private GameObject roomObj;
    private Vector2Int matrixPos;
    private int roomSpace = 1;

    private void Awake()
    {
        //roomObj = new GameObject("Room");
        ///roomObj.transform.SetParent(transform);

        //CreatFloor();
        //CreateWall();
        //CreateDoor();
    }

    public void Initialize(TileMap tileMap, Vector2Int pos)
    {
        roomObj = new GameObject("Room");
        roomObj.transform.SetParent(transform);
        this.tileMap = tileMap;
        this.matrixPos = pos;
        MakeSize();
    }

    public void MakeSize()
    {
        roomSize = new Vector2Int(Random.Range(minRoomHeight, maxRoomHeight), Random.Range(mixRoomWidth, maxRoomWidth));
    }

    public void CreateWall()
    {
        GameObject floorObjects = new GameObject("Floors");
        floorObjects.transform.SetParent(roomObj.transform);
        for (int i = 0; i < roomSize.x; i++)
        {
            for (int j = 0; j < roomSize.y; j++)
            {
                Vector2Int pos = new Vector2Int((int)transform.position.x + i, (int)transform.position.z + j);
                if (tileMap.TryGetTile(pos, out Tile tile) && tile.HasEmptySide())
                {
                    List<Tile> tiles = tile.GetEmptyNeighbors();
                    foreach (Tile item in tiles)
                    {
                        GameObject wallObj = Instantiate(wallTile);
                        wallObj.transform.SetParent(floorObjects.transform);
                        item.SetTileObject(wallObj);
                        item.SetTileType(TileType.Wall);
                    }
                }
            }
        }
    }

    public void CreatFloor()
    {
        GameObject wallObjects = new GameObject("Walls");
        wallObjects.transform.SetParent(roomObj.transform);
        for (int i = 0; i < roomSize.x; i++)
        {
            for (int j = 0; j < roomSize.y; j++)
            {
                Tile tile = new Tile();
                Vector2Int pos = new Vector2Int((int)transform.position.x + i, (int)transform.position.z + j);
                tileMap.CreateTile(pos, tile);

                GameObject floorObj = Instantiate(floorTile);
                floorObj.transform.SetParent(wallObjects.transform);
                tile.SetTileObject(floorObj);
                tile.SetTileType(TileType.Floor);
            }
        }
    }

    public void CreateDoor()
    {
        List<Tile> tileToDoor = new List<Tile>();
        for (int i = 0; i < roomSize.x; i++)
        {
            for (int j = 0; j < roomSize.y; j++)
            {
                Vector2Int pos = new Vector2Int((int)transform.position.x + i, (int)transform.position.z + j);
                if(tileMap.TryGetTile(pos, out Tile tile) && tile.GetNeighborsByType(TileType.Floor).Count == 2)
                {
                    tileToDoor.Add(tile);
                }
            }
        }

        Tile door = tileToDoor[Random.Range(0, tileToDoor.Count)];
        GameObject doorObj = Instantiate(doorTile);
        doorObj.transform.SetParent(transform);
        door.ChangeTileObject(doorObj);
    }

    #region [Getter / Setter]
    public Vector2Int GetPosition()
    {
        return new Vector2Int((int)transform.position.x, (int)transform.position.z);
    }

    public Vector2Int GetCenter()
    {
        Vector2Int center = new Vector2Int((int)transform.position.x + (int)(roomSize.x / 2), (int)transform.position.z + (int)(roomSize.y / 2));
        return center;
    }

    public Tile GetCenterTile()
    {
        Vector2Int center = new Vector2Int((int)transform.position.x + (int)(roomSize.x / 2), (int)transform.position.z + (int)(roomSize.y / 2));
        if(tileMap.TryGetTile(center, out Tile tile))
        {
            return tile;
        }
        return null;
    }

    public Vector2Int GetMatrixPosition()
    {
        return matrixPos;
    }

    public int GetRoomSpace()
    {
        return roomSpace;
    }

    public void SetRoomSpace(int value)
    {
        roomSpace = value;
    }

    public Vector2Int GetRoomSize()
    {
        return roomSize;
    }
    #endregion
}
