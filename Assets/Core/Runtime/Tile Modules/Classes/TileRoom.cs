using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
    private List<Entity> templates;

    [SerializeField]
    private List<string> stopEntities = new List<string> { "room", "wall" };

    [SerializeField]
    private List<Vector3> rayPos;

    [SerializeField]
    private List<Vector3> hits;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private int minCapacity = 16;

    //Stored required components.
    private RoomData roomData;
    private TileMap tileMap;
    private GameObject roomObj;
    private GameObject zonesObj;
    private DoorController doorController;

    //Stored required properties.
    private List<Entity> entities = new List<Entity>();
    private HashSet<Vector2Int> roomTiles;
    private Vector2Int roomSize = Vector2Int.one;
    private Vector2Int matrixPos;
    private string roomName;
    private int roomSpace = 1;

    public void Initialize(RoomData data)
    {
        roomTiles = new HashSet<Vector2Int>();
        hits = new List<Vector3>();
        rayPos = new List<Vector3>();
        foreach (Entity item in templates)
        {
            item.name = item.name.Split()[0];
        }

        roomData = data;
    }

    public void Initialize(TileMap tileMap, Vector2Int pos)
    {
        roomObj = new GameObject("Room");
        roomObj.transform.SetParent(transform);
        this.tileMap = tileMap;
        this.matrixPos = pos;
    }

    public void CalculateSize()
    {
        int capacity = 0;
        List<TextEntity> textEntities = roomData.textEntities;
        foreach (TextEntity tEntity in textEntities)
        {
            List<Entity> entities = templates.Where(x => x.name.ToLower() == tEntity.name.ToLower()).ToList();
            foreach (Entity item in entities)
            {
                capacity += item.GetSize().x * item.GetSize().y;
            }
        }

        capacity = capacity > minCapacity ? capacity : minCapacity;
        int side = (int)Mathf.Sqrt(capacity) + 1;
        roomSize = new Vector2Int(side, side);
    }

    public void CreateWall()
    {
        GameObject newEntity = new GameObject("Wall");
        newEntity.transform.SetParent(zonesObj.transform);
        Entity entity = newEntity.AddComponent<Entity>();
        entity.transform.position = new Vector3((int)transform.position.x, tileMap.transform.position.y, (int)transform.position.z);

        List<Vector2Int> vectors = new List<Vector2Int>();
        GameObject wallObjects = new GameObject("Walls");
        wallObjects.transform.SetParent(roomObj.transform);
        for (int i = 0; i < roomSize.x; i++)
        {
            for (int j = 0; j < roomSize.y; j++)
            {
                int x = (int)Mathf.Min(transform.position.x + i, (transform.position.x + i) / 1);
                int y = (int)Mathf.Min(transform.position.z + j, (transform.position.z + j) / 1);

                Vector2Int pos = new Vector2Int(x, y);
                if (tileMap.TryGetTile(pos, out Tile tile) && tile.HasEmptySide() && tile.GetTileObject() != null)
                {
                    List<Tile> tiles = tile.GetEmptyNeighbors();
                    foreach (Tile item in tiles)
                    {
                        GameObject wallObj = Instantiate(wallTile);
                        wallObj.transform.SetParent(wallObjects.transform);
                        item.SetTileObject(wallObj);
                        item.SetTileType(TileType.Wall);
                        vectors.Add(item.GetPosition());
                        roomTiles.Add(item.GetPosition());
                    }
                }
            }
        }

        entity.SetTiles(vectors);
        entities.Add(entity);
    }

    public void CreatFloor()
    {
        zonesObj = new GameObject("Zones");
        zonesObj.transform.SetParent(transform);

        GameObject newEntity = new GameObject("Room");
        newEntity.transform.SetParent(zonesObj.transform);
        Entity entity = newEntity.AddComponent<Entity>();
        entity.transform.position = new Vector3((int)transform.position.x, tileMap.transform.position.y, (int)transform.position.z);

        List<Vector2Int> vectors = new List<Vector2Int>();
        GameObject wallObjects = new GameObject("Floors");
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
                vectors.Add(tile.GetPosition());
                roomTiles.Add(tile.GetPosition());
            }
        }

/*        for (int i = 0; i < roomSize.x; i++)
        {
            if (i < (int)(roomSize.x / 2))
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
                    vectors.Add(tile.GetPosition());
                    roomTiles.Add(tile.GetPosition());
                }
            }
            else
            {
                for (int j = 0; j < (int)(roomSize.y / 2) + 1; j++)
                {
                    int offset = (roomSize.y - ((int)(roomSize.y / 2) + 1)) / 2;

                    Tile tile = new Tile();
                    Vector2Int pos = new Vector2Int((int)transform.position.x + i, (int)transform.position.z + j + offset);
                    tileMap.CreateTile(pos, tile);

                    GameObject floorObj = Instantiate(floorTile);
                    floorObj.transform.SetParent(wallObjects.transform);
                    tile.SetTileObject(floorObj);
                    tile.SetTileType(TileType.Floor);
                    vectors.Add(tile.GetPosition());
                    roomTiles.Add(tile.GetPosition());
                }
            }
        }*/

        entity.SetTiles(vectors);
        entities.Add(entity);
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
        doorController = doorObj.GetComponent<DoorController>();
    }

    public void InitializeRoomEntities()
    {
        foreach (Entity entity in entities)
        {
            entity.CalculateZones(tileMap, this);
        }
    }

    public IEnumerator SetUpEntities()
    {
        foreach (TextEntity tEntity in roomData.textEntities)
        {
            if (stopEntities.Contains(tEntity.name)) continue;

            List<Entity> tmp = templates.Where(x => x.name == tEntity.name).ToList();
            Entity template = tmp[Random.Range(0, tmp.Count)];
            Entity entity = Instantiate(template);
            entity.transform.SetParent(transform);
            entity.SetTextEntity(tEntity);
            entity.name = tEntity.name.ToLower();
            entities.Add(entity);
        }

        foreach (TextEntity textEntity in roomData.textEntities)
        {
            if (stopEntities.Contains(textEntity.name)) continue;
            if (!string.IsNullOrEmpty(textEntity.parentObj))
            {
                if (entities.Any(x => x.name.ToLower() == textEntity.parentObj.ToLower()))
                {
                    Entity a = entities.Where(x => x.name == textEntity.name).FirstOrDefault();
                    List<Entity> parentEntities = entities.Where(x => x.name.ToLower() == textEntity.parentObj).ToList();
                    Entity b = parentEntities[Random.Range(0, parentEntities.Count)];
                    EntityZone2 zone2 = b.GetZoneByTag(textEntity.position);

                    Vector2Int parentPos = zone2.GetRandomPosition(tileMap, a.GetSize());
                    Vector3 targetPos = new Vector3(parentPos.x - 0.5f, tileMap.transform.position.y, parentPos.y - 0.5f);
                    if (textEntity.position == "on")
                    {
                        Vector3 origin = new Vector3(parentPos.x, tileMap.transform.position.y + 5f, parentPos.y);
                        Ray ray = new Ray(origin, Vector3.down);
                        rayPos.Add(origin);
                        if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f))
                        {
                            targetPos = new Vector3(targetPos.x, hitInfo.point.y, targetPos.z);
                            hits.Add(hitInfo.point);
                        }
                    }

                    a.transform.position = targetPos;
                    a.CalculateTiles(tileMap, parentPos);
                    a.CalculateZones(tileMap, this);

                    List<Vector2Int> vectors = a.GetTiles();
                    foreach (Vector2Int item in vectors)
                    {
                        tileMap.GetTile(item).SetTileContent(a.gameObject);
                    }

                    yield return null;
                }
            }
        }

        foreach (Entity item in entities)
        {
            TextEntity textEntity = item.GetTextEntity();
            if (textEntity != null)
            {
                if (textEntity.interactable)
                {
                    item.GetComponent<InteractiveObject>().IsEnable(true);
                }

                if (textEntity.grabbable)
                {
                    item.gameObject.AddComponent<GrabbableObject>();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        foreach (var item in rayPos)
        {
            Handles.DrawSolidDisc(item, Vector3.up, 0.1f);
            Handles.DrawLine(item, item + (Vector3.down * 10f));
        }

        Handles.color = Color.red;
        foreach (var item in hits)
        {
            Handles.DrawSolidDisc(item, Vector3.up, 0.1f);
            Handles.DrawLine(item, item + (Vector3.down * 10f));
        }
    }

    #region [Getter / Setter]
    public HashSet<Vector2Int> GetRoomTiles()
    {
        return roomTiles;
    }
    
    public DoorController GetDoorController()
    {
        return doorController;
    }

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

    public string GetRoomName()
    {
        return roomName;
    }

    public void SetRoomName(string value)
    {
        roomName = value;
    }
    #endregion
}
