using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    //Stored required components.
    private Transform playertransform;

    //Stored required properties.
    [SerializeField]
    private Vector3 point1;

    [SerializeField]
    private Vector3 point2;

    private void Awake()
    {
        playertransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Initialize(TileMap tileMap, Vector2Int pos)
    {
        List<Tile> tiles = tileMap.GetNeighbors(pos);

        tiles = tiles.Where(x => x.GetTileType() != TileType.Wall).ToList();
        point1 = new Vector3(tiles[0].GetPosition().x, tileMap.transform.position.y, tiles[0].GetPosition().y);
        point2 = new Vector3(tiles[1].GetPosition().x, tileMap.transform.position.y, tiles[1].GetPosition().y);
    }

    public void Teleport()
    {
        float lenght1 = Vector3.Distance(point1, playertransform.position);
        float lenght2 = Vector3.Distance(point2, playertransform.position);

        playertransform.position = lenght1 >= lenght2 ? point1 : point2;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(point1, 0.1f);
        Gizmos.DrawSphere(point2, 0.1f);
    }
}
