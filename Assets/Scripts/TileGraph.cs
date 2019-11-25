using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TileGraph : MonoBehaviour
{
    /// <summary>
    /// The instance of the class.
    /// </summary>
    /// <value></value>
    public static TileGraph Instance { get; private set; }

    /// <summary>
    /// All the tiles in the level.
    /// </summary>
    /// <value></value>
    public List<Tile> Tiles { get; protected set; }

    public Dictionary<Tile, Node<Tile>> Nodes { get; protected set; }

    [SerializeField] readonly bool DebugLog = false;

    [SerializeField] private bool _drawLinesBetweenNodes = false;

    /// <summary>
    /// Called before Start.
    /// </summary>
    private void Awake()
    {
        // Set the singleton instance
        Instance = this;

        Tiles = new List<Tile>();
        // Loop through all the tiles. They should be the only children.
        foreach (Transform tileTransform in transform)
        {
            // Get the tile component and add it to our list
            Tile tile = tileTransform.gameObject.GetComponent<Tile>();
            if (tile != null && tile.Type != Tile.TileType.Wall)
            {
                Tiles.Add(tile);
            }
        }
        UpdateTileGraph();
    }

    private void Update()
    {
        if (_drawLinesBetweenNodes)
        {
            // Draw lines between nodes.
            foreach (Node<Tile> node in Nodes.Values)
            {
                foreach (Edge<Tile> edge in node.Edges)
                {
                    Debug.DrawLine(
                        node.Data.NodeTransform.position,
                        edge.Node.Data.NodeTransform.position,
                        Color.white
                    );
                }
            }
        }
    }

    /// <summary>
    /// Returns the tile nearest to a given position.
    /// </summary>
    /// <param name="position">
    /// The position to which the tile should be nearest.
    /// </param>
    /// <returns>The tile nearest to position.</returns>
    public Tile NearestTile(Vector3 position)
    {
        float leastSqrDistance = Mathf.Infinity;
        Tile nearestTile = Tiles[0];
        foreach (Tile tile in Tiles)
        {
            Vector3 distance = position - tile.NodeTransform.position;
            if (distance.sqrMagnitude < leastSqrDistance)
            {
                leastSqrDistance = distance.sqrMagnitude;
                nearestTile = tile;
            }
        }

        return nearestTile;
    }

    public List<Node<Tile>> ConstructPath(Tile start, Tile goal)
    {
        if (Nodes.ContainsKey(start) && Nodes.ContainsKey(goal))
        {
            List<Node<Tile>> nodeList = AStar.ConstructPath(
                Nodes[start],
                Nodes[goal],
                (node) =>
                {
                    return (node.Data.NodeTransform.position -
                            goal.NodeTransform.position).sqrMagnitude;
                }
            );

            if (nodeList != null)
            {
                /*List<Vector2Int> path = new List<Vector2Int>();
                foreach (var node in nodeList)
                {
                    path.Add(node.Data.Position);
                }*/
                return nodeList;
            }
            else // Error
            {
                Debug.LogError("A* failed to return a valid path.");
                return null;
            }
        }
        else // Error
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


        //for all tiles in the level
        foreach (Tile tile in Tiles)
        {
            Node<Tile> newNode = new Node<Tile>();
            newNode.Data = tile;
            Nodes.Add(tile, newNode);
        }

        //loop through all nodes and create edges for neighbors
        foreach (Node<Tile> node in Nodes.Values)
        {
            List<Tile> neighborList = node.Data.FindNeighbors();
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
                            "(" + tile + ") not in Node list"
                        );
                    }
                }
                neighborNode.Data = tile;
                Edge<Tile> edge = new Edge<Tile>(
                    1,
                    Nodes[tile]
                );
                node.Edges.Add(edge);
            }
        }
    }
}
