using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    public string roomName;
    public List<TextEntity> textEntities;
    public Dictionary<string,string> connections;
    public RoomGenerator RoomGenerator;
    public TileRoom tileRoom;
}
