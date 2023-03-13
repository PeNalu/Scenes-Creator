using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarSearchPF
{
    public static List<Tile> Searching(Tile startTile, Tile targetTile)
    {
        List<Tile> toSearch = new List<Tile>() { startTile };
        List<Tile> processed = new List<Tile>();

        while (toSearch.Count > 0)
        {
            Tile current = toSearch[0];

/*            foreach (Tile t in toSearch)
            {
                if (CompareCost(current, t) > 0)
                {
                    current = t;
                }
            }*/

            processed.Add(current);
            toSearch.Remove(current);

            if (current.GetPosition() == targetTile.GetPosition())
            {
                Tile currentPathTile = targetTile;
                List<Tile> path = new List<Tile>();
                while (currentPathTile != startTile)
                {
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.GetConnect();
                }

                return path;
            }

            foreach (Tile neighbor in current.GetNeighbors().Where(t => !processed.Contains(t)))
            {
                bool inSearch = toSearch.Contains(neighbor);

                float costToNeighbor = 1;

                if (!inSearch || costToNeighbor < 1)
                {
                    neighbor.G = costToNeighbor;
                    neighbor.H = neighbor.GetDistance(targetTile.GetPosition());
                    neighbor.SetConnect(current);

                    if (!inSearch)
                    {
                        toSearch.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }
}
