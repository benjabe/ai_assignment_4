using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType TileType { get; protected set; }
    public Inventory Inventory { get; protected set; }

    public Action onSetTileType;
    public Action onInventoryChanged;

    public Vector2Int Position
    {
        get
        {
            return new Vector2Int(
                (int)Mathf.Round(transform.position.x),
                (int)Mathf.Round(transform.position.y)
            );
        }
    }


    void Awake()
    {
        Inventory = new Inventory();
    }

    public void SetTileType(TileType tileType)
    {
        TileType = tileType;
        onSetTileType?.Invoke();
    }

    /// <summary>
    /// Finds all walkable neighbors and returns them as a list
    /// </summary>
    /// <returns>List of tiles </returns>
    public List<Tile> GetNeighborList()
    {
        World world = World.Instance;
        //Go through all candidate neighbors, return a list of walkable ones
        //Warning: Disturbing imagery ahead
        List<Tile> neighborList = new List<Tile>();
        int X = (int)Mathf.Round(transform.position.x);
        int Y = (int)Mathf.Round(transform.position.y);

        Vector2Int[] positions =
        {
            new Vector2Int(X, Y + 1),
            //new Vector2Int(X + 1, Y + 1),
            new Vector2Int(X + 1, Y),
            //new Vector2Int(X + 1, Y - 1),
            new Vector2Int(X, Y - 1),
            //new Vector2Int(X - 1, Y - 1),
            new Vector2Int(X - 1, Y),
            //new Vector2Int(X - 1, Y + 1)
        };
        foreach (Vector2Int position in positions)
        {
            Tile tile = GetNeighbor(position);
            if (tile != null)
            {
                neighborList.Add(tile);
            }
        }
        return neighborList;
    }

    private Tile GetNeighbor(Vector2Int position)
    {
        if (World.Instance.InBounds(position))
        {
            Tile tile = World.Instance.Tiles[position];
            if (tile.TileType.walkable)
            {
                return tile;
            }
        }
        return null;
    }

    public void AddItems(Dictionary<Item, int> items)
    {
        Inventory.AddItems(items);
        onInventoryChanged();
    }

    public void AddItems(Item item, int quantity)
    {
        Inventory.AddItems(item, quantity);
        onInventoryChanged();
    }

    public void RemoveAllItems()
    {
        Inventory.RemoveAllItems();
        onInventoryChanged();
    }

    public override string ToString()
    {
        return base.ToString() + " : " + Position;
    }
}
