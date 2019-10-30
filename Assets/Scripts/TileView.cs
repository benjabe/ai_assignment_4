using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Tile))]
public class TileView : MonoBehaviour
{
    Tile _tile = null;
    SpriteRenderer _spriteRenderer;
    ItemView _itemView;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _tile = GetComponent<Tile>();
        _tile.onSetTileType += OnSetTileType;
        _tile.onInventoryChanged += OnChangeTileInventory;
        _itemView = GetComponentInChildren<ItemView>();
    }

    private void OnSetTileType()
    {
        _spriteRenderer.sprite = _tile.TileType.sprite;
        _spriteRenderer.color = _tile.TileType.color;
        //Debug.Log("Tile at " + transform.position + "'s type changed to " + tile.name);
    }

    private void OnChangeTileInventory()
    {
        Dictionary<Item, int> items = _tile.Inventory.Items;
        Item mostValueableItem = null;
        foreach(Item item in items.Keys)
        {
            if (mostValueableItem == null ||
                items[item] > items[mostValueableItem])
            {
                mostValueableItem = item;
            }
        }
        if (mostValueableItem != null)
        {
            _itemView.SetItem(mostValueableItem, items[mostValueableItem]);
        }
        else
        {
            _itemView.SetItem(null, 0);
        }
    }
}
