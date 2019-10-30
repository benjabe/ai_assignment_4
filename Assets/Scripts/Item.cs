using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[
    CreateAssetMenu(
        fileName = "Item",
        menuName = "ScriptableObjects/Item",
        order = 1
)]
public class Item : ScriptableObject
{
    [SerializeField] public ItemCategory Category;
    [SerializeField] public string Name = "Item";
    [SerializeField] public string Description = "This is an item";
    [SerializeField] public Sprite sprite = null;

    public enum ItemCategory
    {
        A,
        B
    }

    public static Item GetItemWithName(string itemName)
    {
        string path = "Assets/ScriptableObjects/Items/" + itemName;
        Debug.Log(path);
        return (Item)AssetDatabase.LoadAssetAtPath(path, typeof(Item));
    }
}
