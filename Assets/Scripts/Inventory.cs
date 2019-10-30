using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public Dictionary<Item, int> Items { get; protected set; }

    public int Value
    {
        get
        {
            if (Items == null || Items.Count == 0)
            {
                return 0;
            }
            int result = 0;
            foreach (Item item in Items.Keys)
            {
                result += Items[item];
            }
            return result;
        }
    }

    public Inventory()
    {
        Items = new Dictionary<Item, int>();
    }

    public void AddItems(Item item, int quantity)
    {
        if (!Items.ContainsKey(item))
        {
            Items.Add(item, quantity);
        }
        else
        {
            Items[item] += quantity;
        }
    }

    public void AddItems(Dictionary<Item, int> items)
    {
        foreach (Item item in items.Keys)
        {
            if (!Items.ContainsKey(item))
            {
                Items.Add(item, items[item]);
            }
            else
            {
                Items[item] += items[item];
            }
        }
    }

    public void RemoveItems(Item item, int quantity)
    {
        if (Items.ContainsKey(item))
        {
            if (Items[item] >= quantity)
            {
                Items[item] -= quantity;
                if (Items[item] <= 0)
                {
                    Items.Remove(item);
                }
            }
            else
            {
                Debug.Log("Not enough items");
            }
        }
        else
        {
            Debug.Log("Item " + item.name + " not in inventory.\n");
        }
    }

    public void RemoveAllItems()
    {
        Items = new Dictionary<Item, int>();
    }
}
