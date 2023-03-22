using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Voxell.Inspector;

public partial class ApartmentGenerator : MonoBehaviour
{
    [SerializeField]
    private TextAsset textAsset;

    [SerializeField]
    private List<string> roomName;

    [SerializeField]
    private List<string> connectedMark;

    [SerializeField]
    private NLPSentenceSplitter sentenceSplitter;

    [SerializeField]
    private NLPParser parser;

    [SerializeField]
    private RoomGenerator roomGeneratorTemplate;

    [SerializeField]
    private RoomsCreator roomsCreator;

    [SerializeField]
    private TileRoom tileRoom;

    [SerializeField]
    private TileMap tileMap;

    //Stored required components.
    private List<TileRoom> tileRooms;

    //Stored required properties.
    private Dictionary<string, RoomData> roomDatas;
    private Dictionary<Vector2Int, TileRoom> roomsMap;
    private List<RoomConnectorData> roomConnecters = new List<RoomConnectorData>();
    private List<string> roomNames = new List<string>();
    private List<string> createdRooms;
    private bool flag = true;

    [Button]
    public void GenerateApartment()
    {
        ResetFields();
        string[] sentences = sentenceSplitter.GetSentence(textAsset.text);
        List<string> roomDescriptions = new List<string>();

        FindRooms(sentences, roomDescriptions);
        SetUpRooms(roomDescriptions);
        ConfigurateRoomsPosition();

        roomsCreator.Clear();
        foreach (TileRoom item in tileRooms)
        {
            roomsCreator.AddRoom(item);
        }
        roomsCreator.StartCreate(transform);

        foreach (Tile door in roomsCreator.GetDoors())
        {
            DoorController doorController = door.GetTileObject().GetComponent<DoorController>();
            doorController.Initialize(tileMap, door.GetPosition());
        }
    }

    [Button]
    private void RemoveApartment()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform transformChild = transform.GetChild(i);
            DestroyImmediate(transformChild.gameObject);
        }
        roomNames = new List<string>();
    }

    private void ConfigurateRoomsPosition()
    {
        foreach (RoomConnectorData connectorData in roomConnecters)
        {
            roomDatas[connectorData.FirstRoomName].connections.Add(connectorData.SecondRoomName, connectorData.ConnectionMethod);
            roomDatas[connectorData.SecondRoomName].connections.Add(connectorData.FirstRoomName, connectorData.ConnectionMethod);
        }

        RoomData firstRoom = roomDatas.First().Value;
        TileRoom newTileRoom = Instantiate(tileRoom);
        newTileRoom.Initialize(tileMap, Vector2Int.zero);
        newTileRoom.SetRoomName(firstRoom.roomName);
        newTileRoom.Initialize(roomDatas[firstRoom.roomName]);
        newTileRoom.CalculateSize();
        createdRooms.Add(firstRoom.roomName);
        roomsMap.Add(Vector2Int.zero, newTileRoom);
        firstRoom.tileRoom = newTileRoom;
        tileRooms.Add(newTileRoom);
        newTileRoom.transform.SetParent(transform);
        newTileRoom.transform.position = new Vector3(0, tileMap.transform.position.y, 0);

        foreach (KeyValuePair<string, RoomData> item in roomDatas)
        {
            foreach (KeyValuePair<string, string> conn in item.Value.connections)
            {
                if (!createdRooms.Contains(conn.Key))
                {
                    if (HasEmptySide(item.Value.tileRoom.GetMatrixPosition()))
                    {
                        Vector2Int pos = GetRandomEmptySide(item.Value.tileRoom.GetMatrixPosition());
                        TileRoom tRoom = Instantiate(tileRoom);
                        tRoom.Initialize(tileMap, pos);
                        tRoom.SetRoomName(conn.Key);
                        tRoom.Initialize(roomDatas[conn.Key]);
                        tRoom.CalculateSize(); 
                        createdRooms.Add(item.Value.roomName);
                        roomsMap.Add(pos, tRoom);
                        tileRooms.Add(tRoom);
                        roomDatas[conn.Key].tileRoom = tRoom;
                        tRoom.transform.SetParent(transform);

                        roomsCreator.OnCreateHallway += () =>
                        {
                            roomsCreator.MakeHallway(item.Value.tileRoom.GetCenterTile(), tRoom.GetCenterTile(), transform);
                        };

                        if (conn.Value == "connected by a corridor")
                        {
                            tRoom.SetRoomSpace(Random.Range(3, 9));
                        }

                        Vector2Int matPos = item.Value.tileRoom.GetMatrixPosition();
                        Vector3 position = Vector3.zero;
                        int y, x = 0;
                        if (pos.x == matPos.x)
                        {
                            y = pos.y < matPos.y ? item.Value.tileRoom.GetPosition().y - (tRoom.GetRoomSize().y + tRoom.GetRoomSpace()) 
                                : item.Value.tileRoom.GetPosition().y + (item.Value.tileRoom.GetRoomSize().y + tRoom.GetRoomSpace());
                            position = new Vector3(item.Value.tileRoom.GetPosition().x, tileMap.transform.position.y, y);
                        }
                        else
                        {
                            x = pos.x < matPos.x ? item.Value.tileRoom.GetPosition().x - (tRoom.GetRoomSize().x + tRoom.GetRoomSpace())
                                : item.Value.tileRoom.GetPosition().x + (tRoom.GetRoomSize().x + tRoom.GetRoomSpace());
                            position = new Vector3(x, tileMap.transform.position.y, item.Value.tileRoom.GetPosition().y);
                        }
                        tRoom.transform.position = position;
                    }
                    else
                    {
                        Debug.LogError($"The {item.Value.roomName} room has no connection points.");
                    }
                }
            }
        }
    }

    private void SetUpRooms(List<string> roomDescriptions)
    {
        for (int i = 0; i < roomDescriptions.Count; i++)
        {
            string roomDescription = roomDescriptions[i];
            RoomGenerator roomGenerator = Instantiate(roomGeneratorTemplate, transform.position, Quaternion.identity);
            roomGenerator.name = "room";
            roomGenerator.Initialize(parser, roomNames[i]);
            roomGenerator.transform.SetParent(transform);
            RoomData roomData = new RoomData();
            roomData.roomName = roomNames[i];
            roomData.textEntities = roomGenerator.GetTextEntities(roomDescription);
            roomData.RoomGenerator = roomGenerator;
            roomData.connections = new Dictionary<string, string>();
            roomDatas.Add(roomNames[i], roomData);
        }
    }

    private void FindRooms(string[] sentences, List<string> roomDescriptions)
    {
        StringBuilder description = new StringBuilder();
        foreach (string sentance in sentences)
        {
            if (!HasConnected(sentance))
            {
                flag = true;
                foreach (string room in roomName)
                {
                    if (sentance.Contains(room))
                    {
                        string str = description.ToString();
                        if (!string.IsNullOrEmpty(str))
                        {
                            roomDescriptions.Add(str);
                        }

                        roomNames.Add(room);
                        description.Clear();
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    description.Append(sentance);
                }
            }
        }
        string lastSentance = description.ToString();
        if (!string.IsNullOrEmpty(lastSentance))
        {
            roomDescriptions.Add(lastSentance);
        }
    }

    private void ResetFields()
    {
        RemoveApartment();
        tileMap.Initialize();
        roomsCreator.Initialize();
        tileRooms = new List<TileRoom>();
        createdRooms = new List<string>();
        roomsMap = new Dictionary<Vector2Int, TileRoom>();
        roomDatas = new Dictionary<string, RoomData>();
        roomConnecters = new List<RoomConnectorData>();
    }

    private bool HasEmptySide(Vector2Int pos)
    {
        List<Vector2Int> vectors = GetEmptySide(pos);
        return vectors.Count > 0;
    }

    private List<Vector2Int> GetEmptySide(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        List<Vector2Int> vectors = new List<Vector2Int>()
        {
        new Vector2Int(pos.x, pos.y + 1),
        new Vector2Int(pos.x, pos.y - 1),
        new Vector2Int(pos.x - 1, pos.y),
        new Vector2Int(pos.x + 1, pos.y)
        };

        foreach (Vector2Int item in vectors)
        {
            if (!roomsMap.ContainsKey(item))
            {
                neighbors.Add(item);
            }
        }

        return neighbors;
    }

    private Vector2Int GetRandomEmptySide(Vector2Int pos)
    {
        List<Vector2Int> vectors = GetEmptySide(pos);
        return vectors[Random.Range(0, vectors.Count)];
    }

    private bool HasConnected(string sentance)
    {
        sentance = sentance.ToLower();
        RoomConnectorData roomConnecter = new RoomConnectorData();
        foreach (string mark in connectedMark)
        {
            if (sentance.Contains(mark))
            {
                roomConnecter.ConnectionMethod = mark;
                break;
            }
        }

        if (string.IsNullOrEmpty(roomConnecter.ConnectionMethod))
        {
            return false;
        }

        bool flag = false;
        int i1 = 0;
        int i2 = 0;
        List<string> rooms = new List<string>();
        foreach (string name in roomName)
        {
            if (sentance.Contains(name))
            {
                if (!flag)
                {
                    i1 = sentance.IndexOf(name);
                    flag = true;
                }
                else
                {
                    i2 = sentance.IndexOf(name);
                }

                rooms.Add(name);
            }
        }

        roomConnecter.FirstRoomName = i1 <= i2 ? rooms[0] : rooms[1];
        roomConnecter.SecondRoomName = i1 <= i2 ? rooms[1] : rooms[0];

/*        roomConnecter.FirstRoomName = rooms[1];
        roomConnecter.SecondRoomName = rooms[0];*/
        roomConnecters.Add(roomConnecter);
        return true;
    }
}
