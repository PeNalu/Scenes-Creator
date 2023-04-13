using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    [SerializeField]
    private Vector2Int size = Vector2Int.zero;

    protected virtual void Awake()
    {
/*        tiles = new List<Vector2Int>();
        if (size != Vector2Int.zero)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int pos = new Vector2Int((int)transform.position.x + x, (int)transform.position.z + y);
                    tiles.Add(pos);
                }
            }
        }*/
    }

    //Stored required properties.
    private List<Vector2Int> tiles;

    //Stored required components.

    public void CalculateTiles(TileMap tileMap, Vector2Int pos)
    {
        tiles = new List<Vector2Int>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int tile = new Vector2Int(pos.x + x, pos.y + y);
                tiles.Add(tile);
            }
        }
    } 

    protected virtual void OnDrawGizmos()
    {
        Vector3 pos = new Vector3((int)transform.position.x, transform.position.y, (int)transform.position.z);
        Gizmos.color = Color.green;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3 offset = new Vector3(x + 0.5f, 2.5f, y + 0.5f);
                Vector3 cubePos = new Vector3(pos.x + offset.x, pos.y + offset.y, pos.z + offset.z);
                Gizmos.DrawWireCube(cubePos, new Vector3(1, 5, 1));
            }
        }
    }

    #region [Getter / Setter]
    public Vector2Int GetSize()
    {
        return size;
    }

    public List<Vector2Int> GetTiles()
    {
        return tiles;
    }

    public void SetTiles(List<Vector2Int> vectors)
    {
        tiles = vectors;
    }
    #endregion
}
