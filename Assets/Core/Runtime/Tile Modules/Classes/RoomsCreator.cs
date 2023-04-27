using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxell.Inspector;

public class RoomsCreator : MonoBehaviour
{
    [SerializeField]
    private TileMap tileMap;

    [SerializeField]
    private GameObject hallwayTile;

    [SerializeField]
    private GameObject doorTile;

    [SerializeField]
    private GameObject wallTile;

    //Stored required properties.

    //Stored required components.
    private List<TileRoom> tileRooms;
    private List<Tile> hallwayTiles;
    private List<Tile> doorTiles;

    public void Initialize()
    {
        OnCreateHallway = null;
    }

    [Button]
    public void StartCreate(Transform parent = null)
    {
        StartCoroutine(CreateProcessing(parent));
    }

    private IEnumerator CreateProcessing(Transform parent = null)
    {
        hallwayTiles = new List<Tile>();
        doorTiles = new List<Tile>();
        foreach (TileRoom room in tileRooms)
        {
            room.CreatFloor();
        }
        OnFloorCreatedCallback?.Invoke();

        OnCreateHallway?.Invoke();

        foreach (TileRoom room in tileRooms)
        {
            room.CreateWall();
        }
        OnWallCreatedCallback?.Invoke();

        foreach (var item in hallwayTiles)
        {
            if (item.HasEmptySide())
            {
                List<Tile> tiles = item.GetEmptyNeighbors();
                foreach (Tile t in tiles)
                {
                    GameObject wallObj = Instantiate(wallTile);
                    if (parent != null)
                    {
                        wallObj.transform.SetParent(parent);
                    }
                    else
                    {
                        wallObj.transform.SetParent(transform);
                    }
                    t.SetTileObject(wallObj);
                    t.SetTileType(TileType.Wall);
                }
            }
        }

        foreach (var item in doorTiles)
        {
            if (item.HasEmptySide())
            {
                List<Tile> tiles = item.GetEmptyNeighbors();
                foreach (Tile t in tiles)
                {
                    GameObject wallObj = Instantiate(wallTile);
                    if (parent != null)
                    {
                        wallObj.transform.SetParent(parent);
                    }
                    else
                    {
                        wallObj.transform.SetParent(transform);
                    }
                    t.SetTileObject(wallObj);
                    t.SetTileType(TileType.Wall);
                }
            }
        }

        yield return null;

        foreach (TileRoom tileRoom in tileRooms)
        {
            tileRoom.InitializeRoomEntities();
            StartCoroutine(tileRoom.SetUpEntities());
        }
    }

    public void MakeHallway(Tile start, Tile target, Transform parent = null)
    {
        List<Tile> hallway = AStarSearchPF.Searching(start, target);
        foreach (Tile item in hallway)
        {
            if (item.GetTileObject() == null)
            {
                if (item.HasNeighborsType(TileType.Floor))
                {
                    GameObject gameObject = Instantiate(doorTile);
                    if(parent == null)
                    {
                        gameObject.transform.SetParent(transform);
                    }
                    else
                    {
                        gameObject.transform.SetParent(parent);
                    }
                    item.SetTileObject(gameObject);
                    item.SetTileType(TileType.Door);
                    doorTiles.Add(item);
                }
                else
                {
                    GameObject gameObject = Instantiate(hallwayTile);
                    if (parent == null)
                    {
                        gameObject.transform.SetParent(transform);
                    }
                    else
                    {
                        gameObject.transform.SetParent(parent);
                    }
                    item.SetTileObject(gameObject);
                    hallwayTiles.Add(item);
                }
            }
        }
    }


    #region [Callback Functions]
    public event Action OnFloorCreatedCallback;
    public event Action OnWallCreatedCallback;
    public event Action OnCreateHallway;
    #endregion

    #region [Getter / Setter]
    public void Clear()
    {
        tileRooms = new List<TileRoom>();
    }

    public void AddRoom(TileRoom tileRoom)
    {
        tileRooms.Add(tileRoom);
    }

    public List<Tile> GetDoors()
    {
        return doorTiles;
    }
    #endregion
}
