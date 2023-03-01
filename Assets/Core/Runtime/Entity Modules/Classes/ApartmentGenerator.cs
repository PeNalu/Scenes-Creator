using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Voxell.Inspector;

public partial class ApartmentGenerator : MonoBehaviour
{
    [TextArea]
    public string textToParse;

    [SerializeField]
    private List<string> roomName;

    [SerializeField]
    private List<string> connectedMark;

    [SerializeField]
    private List<RoomGenerator> roomGenerators;

    [SerializeField]
    private NLPSentenceSplitter sentenceSplitter;

    [SerializeField]
    private NLPParser parser;

    [SerializeField]
    private RoomGenerator roomGeneratorTemplate;

    [SerializeField]
    private DoorController doorTemplate;

    [SerializeField]
    private Transform hallwayTemplate;

    [SerializeField]
    private float roomDiameter = 10f;

    [SerializeField]
    private float hallwayLenght = 10f;

    //Stored required properties.
    private List<string> roomNames = new List<string>();
    private List<RoomConnectorData> roomConnecters = new List<RoomConnectorData>();
    private bool flag = true;

    [Button]
    public void GenerateApartment()
    {
        RemoveApartment();
        roomConnecters = new List<RoomConnectorData>();
        string[] sentences = sentenceSplitter.GetSentence(textToParse);
        List<string> roomDescriptions = new List<string>();
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

        for (int i = 0; i < roomDescriptions.Count; i++)
        {
            string roomDescription = roomDescriptions[i];
            RoomGenerator roomGenerator = Instantiate(roomGeneratorTemplate, transform.position, Quaternion.identity);
            roomGenerator.name = "room";
            roomGenerator.Initialize(parser, roomNames[i]);
            roomGenerator.Generate(roomDescription);
            roomGenerator.transform.SetParent(transform);
            roomGenerators.Add(roomGenerator);
        }

        CreateConnection();
    }

    private void CreateConnection()
    {
        foreach (RoomConnectorData connector in roomConnecters)
        {
            RoomGenerator a = roomGenerators.Where(x => x.GetRoomName() == connector.FirstRoomName).First();
            RoomGenerator b = roomGenerators.Where(x => x.GetRoomName() == connector.SecondRoomName).First();

            bool flag = b.HasConnectedObject();

            RoomConnector roomConnector = a.GetRoomConnectors()[Random.Range(0, a.GetRoomConnectors().Count)];
            while (roomConnector.HasConnection())
            {
                roomConnector = a.GetRoomConnectors()[Random.Range(0, a.GetRoomConnectors().Count)];
            }
            b.transform.position = b.transform.position + (roomConnector.transform.forward * roomDiameter);

            RoomConnector nearest = null;
            nearest = GetNearestConnector(b, roomConnector, nearest);

            Transform connectionObj = CreateConnection(connector, a, b, roomConnector);
            if (flag)
            {
                UpdateRoomPosition(b);
            }

            roomConnector.SetConnectorData(connector);
            nearest.SetConnectorData(connector);

            roomConnector.SetConnection(b.transform);
            nearest.SetConnection(a.transform);

            roomConnector.SetConnectionObject(connectionObj);
            nearest.SetConnectionObject(connectionObj);
        }
    }

    private static RoomConnector GetNearestConnector(RoomGenerator b, RoomConnector roomConnector, RoomConnector closed)
    {
        float dist = float.MaxValue;
        foreach (var item in b.GetRoomConnectors())
        {
            float d = Vector3.Distance(roomConnector.transform.position, item.transform.position);
            if (d < dist)
            {
                dist = d;
                closed = item;
            }
        }

        return closed;
    }

    private Transform CreateConnection(RoomConnectorData connector, RoomGenerator a, RoomGenerator b, RoomConnector roomConnector)
    {
        Transform result = null;
        if (connector.ConnectionMethod == "connected")
        {
            Vector3 connectPos = (a.transform.position + b.transform.position) / 2f;
            DoorController doorController = Instantiate(doorTemplate, connectPos, Quaternion.identity, transform);
            result = doorController.transform;
        }

        if (connector.ConnectionMethod == "connected by a corridor")
        {
            b.transform.position = b.transform.position + (roomConnector.transform.forward * hallwayLenght);
            Vector3 connectPos = (a.transform.position + b.transform.position) / 2f;
            Transform hallwayTranform = Instantiate(hallwayTemplate, connectPos, Quaternion.identity, transform);
            result = hallwayTranform;
        }
        result.LookAt(a.transform);
        return result;
    }

    private void UpdateRoomPosition(RoomGenerator b)
    {
        foreach (RoomConnector item in b.GetRoomConnectors())
        {
            if (item.HasConnection())
            {
                RoomConnectorData roomConnectorData = item.GetConnectorData();
                item.GetChild().position = b.transform.position + (item.transform.forward * roomDiameter);
                if (roomConnectorData.ConnectionMethod == "connected")
                {
                    Vector3 connectPos = (b.transform.position + item.GetChild().transform.position) / 2f;
                    item.GetConnectionObject().position = connectPos;
                }

                if (roomConnectorData.ConnectionMethod == "connected by a corridor")
                {
                    item.GetChild().position = item.GetChild().transform.position + (item.transform.forward * hallwayLenght);
                    Vector3 connectPos = (b.transform.position + item.transform.position) / 2f;
                    item.GetConnectionObject().position = connectPos;
                }
            }
        }
    }

    /*    private void CreateConnection()
        {
            roomConnecters.OrderBy(x => x.ConnectionMethod.Length);
            foreach (RoomConnectorData connector in roomConnecters)
            {
                RoomGenerator a = roomGenerators.Where(x => x.GetRoomName() == connector.FirstRoomName).First();
                RoomGenerator b = roomGenerators.Where(x => x.GetRoomName() == connector.SecondRoomName).First();
                if (a.transform.position == b.transform.position)
                {
                    b.transform.position = b.transform.position + (b.transform.forward * roomDiameter);
                }

                if (connector.ConnectionMethod == "connected")
                {
                    Vector3 connectPos = (a.transform.position + b.transform.position) / 2f;
                    Instantiate(doorTemplate, connectPos, Quaternion.identity, transform);
                }

                if (connector.ConnectionMethod == "connected by a corridor")
                {
                    b.transform.position = b.transform.position + (b.transform.forward * hallwayLenght);

                    Vector3 connectPos = (a.transform.position + b.transform.position) / 2f;
                    Instantiate(hallwayTemplate, connectPos, Quaternion.identity, transform);
                }
            }
        }*/

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

    [Button]
    private void RemoveApartment()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform transformChild = transform.GetChild(i);
            DestroyImmediate(transformChild.gameObject);
        }
        roomGenerators = new List<RoomGenerator>();
        roomNames = new List<string>();
    }
}
