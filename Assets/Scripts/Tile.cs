using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A tile.
/// </summary>
public class Tile : MonoBehaviour
{
    /// <summary>
    /// The tile's type.
    /// </summary>
    [SerializeField] public TileType Type = TileType.DownRamp;

    /// <summary>
    /// The transform of the node's pathfinding node,
    /// (likely) placed on the ground in the centre of the tile.
    /// </summary>
    [SerializeField] private Transform _nodeTransform = null;
    public Transform NodeTransform
    {
        get => _nodeTransform;
    }

    // Awake is called before Start
    void Awake()
    {
        if (NodeTransform == null)
        {
            Debug.LogError(name + " has no node transform!", this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Finds the tile's neighbours and returns them in a list.
    /// </summary>
    /// <returns>The list of nieghbouring tiles.</returns>
    public List<Tile> FindNeighbors()
    {
        // Literally go through every other node in existance
        // and see if they are close enough to be considered neighbours.
        // There might be more efficient ways to do this but for our scope
        // it should be fine since it happens only upon loading the game.

        List<Tile> neighbors = new List<Tile>();

        foreach (Tile tile in TileGraph.Instance.Tiles)
        {
            // If the tile is this then we cannot
            // really consider it a neighbour.
            if (tile != this)
            {
                // Check distance
                Vector3 distance = tile.NodeTransform.position - NodeTransform.position;
                if (distance.sqrMagnitude < 2.0f)
                {
                    // Also check that they aren't too close/stacked
                    if (Mathf.Abs(distance.x) > 0.1f ||
                        Mathf.Abs(distance.z) > 0.1f)
                    {
                        // Check if we can actually move from this tile
                        // to the potential neighbour.
                        if (IsNeighborReachable(tile))
                        {
                            neighbors.Add(tile);
                        }
                    }
                }
            }
        }

        return neighbors;
    }

    /// <summary>
    /// Checks if a ramp leads to a valid connection to a tile.
    /// </summary>
    /// <param name="ramp">The ramp.</param>
    /// <param name="tile">The tile.</param>
    /// <returns>Returns true if you can go from the ramp to the tile.</returns>
    private static bool DoesRampConnectToTile(Tile ramp, Tile tile)
    {
        switch (tile.Type)
        {
        case TileType.DownRamp:
            // A down ramp can connect to a down ramp if
            // they are rotated the same way and the connection
            // is aligned with the rotation.
            float rotationDifference =
                Mathf.Abs(ramp.transform.eulerAngles.y) -
                Mathf.Abs(tile.transform.eulerAngles.y);
            if (Mathf.Abs(rotationDifference) <= 5.0f)
            {
                // If y rotation ~= ~180 or 0
                // They must have the same-ish z location
                if (Mathf.Abs(Mathf.Abs(ramp.transform.eulerAngles.y) - 180.0f) < 5.0f ||
                    Mathf.Abs(ramp.transform.eulerAngles.y) < 5.0f)
                {
                    float distanceZ = Mathf.Abs(
                        tile.NodeTransform.position.z -
                        ramp.NodeTransform.position.z
                    );
                    return distanceZ < 0.1f;
                }

                // If the rotation ~= 90 or 270
                // The must have the same-ish x location
                if (Mathf.Abs(Mathf.Abs(ramp.transform.eulerAngles.y) - 90.0f) < 5.0f)
                {
                    float distanceX = Mathf.Abs(
                        tile.NodeTransform.position.x -
                        ramp.NodeTransform.position.x
                    );
                    return distanceX < 0.1f;
                }
            }
            return false;
        case TileType.Floor:
            // We'll assume that no ramps are dead ends, so any floor
            // that aligns with its rotation is considered valid.

            // If y rotation ~= ~180 or 0
            // They must have the same-ish z location
            if (Mathf.Abs(Mathf.Abs(ramp.transform.eulerAngles.y) - 180.0f) < 5.0f ||
                Mathf.Abs(ramp.transform.eulerAngles.y) < 5.0f)
            {
                float distanceZ = Mathf.Abs(
                    tile.NodeTransform.position.z -
                    ramp.NodeTransform.position.z
                );
                return distanceZ < 0.1f;
            }

            // If the rotation ~= 90 or 270
            // The must have the same-ish x location
            if (Mathf.Abs(Mathf.Abs(ramp.transform.eulerAngles.y) - 90.0f) < 5.0f)
            {
                float distanceX = Mathf.Abs(
                    tile.transform.position.x -
                    ramp.transform.position.x
                );
                return distanceX < 0.1f;
            }
            return false;
        default:
            return false;
        }
    }

    /// <summary>
    /// Checks if two tiles nodes are the same-ish height.
    /// </summary>
    /// <param name="floor1">The first floor to be compared.</param>
    /// <param name="floor2">The second floor to be compared.</param>
    /// <returns>Returns true if both floors are roughly on the same y level.</returns>
    private bool FloorsAreSameHeight(Tile floor1, Tile floor2)
    {
        float yDistance = Mathf.Abs(
            floor1.NodeTransform.position.y -
            floor2.NodeTransform.position.y
        );

        return yDistance <= 0.1f;
    }

    /// <summary>
    /// Checks if a neighbouring tile is reachable.
    /// </summary>
    /// <param name="neighbor">The neighbour to check.</param>
    /// <returns>Returns true if he neighbouring tile is reachable.</returns>
    private bool IsNeighborReachable(Tile neighbor)
    {
        // Compare the tile type of this node with the
        // tile type of the neighbour and assess its reachability.
        switch (Type)
        {
        case TileType.DownRamp:
            return DoesRampConnectToTile(this, neighbor);
        case TileType.Floor:
            switch (neighbor.Type)
            {
            case TileType.DownRamp:
                return DoesRampConnectToTile(neighbor, this);
            case TileType.Floor:
                return FloorsAreSameHeight(neighbor, this);
            case TileType.HighFloor:
                return FloorsAreSameHeight(neighbor, this);
            default:
                return false;
            }
        case TileType.HighFloor:
            switch (neighbor.Type)
            {
            case TileType.DownRamp:
                return DoesRampConnectToTile(neighbor, this);
            case TileType.Floor:
                return FloorsAreSameHeight(neighbor, this);
            case TileType.HighFloor:
                return FloorsAreSameHeight(neighbor, this);
            default:
                return false;
            }
        //case TileType.LowFloor: Haven't used this so let's not bother
        //case TileType.UpRamp: Same here
        default:
            return false;
        }
    }

    public enum TileType
    {
        DownRamp,
        Floor,
        HighFloor,
        LowFloor,
        UpRamp,
        Wall
    }
}