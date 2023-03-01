using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxell.Inspector;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField]
    private List<Entity> roomEntity;

    [SerializeField]
    private List<Entity> templates;

    [SerializeField]
    private List<Entity> toRemove;

    [SerializeField]
    private List<RoomConnector> roomConnectors;

    //Stored required components.
    [SerializeField]
    private string roomName;
    private NLPParser parser;
    private Dictionary<string, Entity> entities;
    private List<Entity> allEntity;

    public void Initialize(NLPParser parser, string roomName)
    {
        foreach (Entity item in templates)
        {
            item.name = item.name.Split()[0];
        }

        this.parser = parser;
        this.roomName = roomName;
    }

    public void Generate(string roomDescription)
    {
        GenerateRoom(parser.Parse(roomDescription));
    }

    public void GenerateRoom(List<TextEntity> textEntities)
    {
        toRemove = new List<Entity>();
        allEntity = new List<Entity>();
        entities = new Dictionary<string, Entity>();
        foreach (Entity roomEntitty in roomEntity)
        {
            roomEntitty.IsStatic(true);
            allEntity.Add(roomEntitty);

            if (!entities.ContainsKey(roomEntitty.name.ToLower()))
            {
                entities.Add(roomEntitty.name.ToLower(), roomEntitty);
            }
        }

        foreach (TextEntity textEntity in textEntities)
        {
            if (!entities.ContainsKey(textEntity.name))
            {
                if (roomEntity.Where(x => x.name == textEntity.name).FirstOrDefault() == null)
                {
                    List<Entity> temp = templates.Where(x => x.name == textEntity.name).ToList();
                    if (temp == null)
                    {
                        Debug.LogError($"We neet add {textEntity.name} entity template");
                        continue;
                    }

                    Entity template = temp[Random.Range(0, temp.Count)];
                    Entity entity = Instantiate(template);
                    entity.SetTextEntity(textEntity);
                    toRemove.Add(entity);
                    entity.name = template.name.ToLower();
                    entities.Add(template.name.ToLower(), entity);
                    allEntity.Add(entity);
                }
            }
        }

        foreach (TextEntity textEntity in textEntities)
        {
            if (!string.IsNullOrEmpty(textEntity.parentObj))
            {
                if (entities.ContainsKey(textEntity.parentObj))
                {
                    Entity a = entities[textEntity.name];
                    List<Entity> parentEntities = allEntity.Where(x => x.name.ToLower() == textEntity.parentObj).ToList();
                    Entity b = parentEntities[Random.Range(0, parentEntities.Count)];
                    
                    Vector3 pos = b.GetPosition(textEntity.position);
                    a.transform.position = pos;
                    a.transform.SetParent(b.transform);
                    if (!a.IsStatic())
                    {
                        Quaternion quaternion = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                        a.transform.localRotation = quaternion;
                    }
                    else
                    {
                        Vector3 v1 = new Vector3(transform.position.x, 0, transform.position.z);
                        Vector3 v2 = new Vector3(pos.x, 0, pos.z);
                        a.transform.rotation = Quaternion.LookRotation(v1 - v2, Vector3.up);
                        float y = a.transform.eulerAngles.y;
                        float coef = y / 90f;
                        int c = Mathf.FloorToInt(coef);

                        Vector3 rot = new Vector3(a.transform.eulerAngles.x, c*90, a.transform.eulerAngles.z);
                        a.transform.rotation = Quaternion.Euler(rot);
                    }
                }
            }
        }

        foreach (Entity item in allEntity)
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

    public bool HasConnectedObject()
    {
        return roomConnectors.Any(x => x.GetChild() != null);
    }

    [Button]
    public void RemoveEntity()
    {
        if(toRemove != null)
        {
            for (int i = 0; i < toRemove.Count; i++)
            {
                if (toRemove[i] != null)
                {
                    DestroyImmediate(toRemove[i].gameObject);
                }
            }

            toRemove = null;
        }
    }

    #region [Getter / Setter]
    public string GetRoomName()
    {
        return roomName;
    }

    public List<RoomConnector> GetRoomConnectors()
    {
        return roomConnectors;
    }
    #endregion
}
