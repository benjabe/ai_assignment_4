using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TileGraph
{

    //constructs tile graph. Each walkable neighbor is
    //linked by edge.

    public Dictionary<Tile, Node<Tile>> Nodes { get; protected set; }

    public Action OnGraphUpdate;

    [SerializeField] readonly bool DebugLog = false;

    public TileGraph(List<Tile> tiles)
    {
        // Populate graph based based on tiles
        World.Instance.OnTerrainUpdate += UpdateTileGraph;
    }

    public List<Tile> ConstructPath(Tile start, Tile goal)
    {
        if (Nodes.ContainsKey(start) && Nodes.ContainsKey(goal))
        {
            List<Node<Tile>> nodeList
                = AStar.ConstructPath(Nodes[start], Nodes[goal]);

            if (nodeList != null)
            {
                List<Tile> path = new List<Tile>();
                foreach (var node in nodeList)
                {
                    path.Add(node.Data);
                }
                return path;
            }
            else
            {
                if (DebugLog)
                {
                    Debug.LogError("A* failed to return a valid path.");
                }
                return null;
            }
        }
        else
        {
            if (!Nodes.ContainsKey(start))
            {
                if (DebugLog)
                {
                    Debug.LogError("Start tile has no assigned node.");
                }
            }
            if (!Nodes.ContainsKey(goal))
            {
                if (DebugLog)
                {
                    Debug.LogError("Goal tile has no assigned node.");
                }
            }
            return null;
        }
    }

    private void UpdateTileGraph()
    {
        if (DebugLog) { Debug.Log("Building tile graph..."); }
        //Create node dictionary
        Nodes = new Dictionary<Tile, Node<Tile>>();


        //for all tiles in the world
        for (int x = 0; x < World.Instance.WorldSize; x++)
        {
            for (int y = 0; y < World.Instance.WorldSize; y++)
            {
                //get the currently processed tile

                Tile tile =
                    World.Instance.Tiles[new Vector2Int(x, y)];

                if (tile.TileType.walkable) //if the tile is walkable
                {
                    Node<Tile> newNode = new Node<Tile>();
                    newNode.Data = tile;
                    Nodes.Add(tile, newNode);
                }

            }
        }

        //loop through all nodes and create edges for neighbors
        foreach (Node<Tile> node in Nodes.Values)
        {
            List<Tile> neighborList = node.Data.GetNeighborList();
            //find the neighbors of the tile
            //create an edge to each of them
            foreach (Tile tile in neighborList)
            {
                Node<Tile> neighborNode = null;
                if (Nodes.ContainsKey(tile))
                {
                    neighborNode = Nodes[tile];
                }
                else
                {
                    if (DebugLog)
                    {
                        Debug.LogError(
                        "Neighbor of " + node.Data +
                        "(" + tile + ") not in Node dictionary"
                    );
                    }
                }
                neighborNode.Data = tile;
                Edge<Tile> edge = new Edge<Tile>(tile.TileType.moveCost, Nodes[tile]);
                node.Edges.Add(edge);
            }
        }

        OnGraphUpdate?.Invoke();
    }
}
