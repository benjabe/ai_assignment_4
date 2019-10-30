using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[
    CreateAssetMenu(
        fileName = "Tile Type",
        menuName = "ScriptableObjects/Tile Type",
        order = 1
)]
public class TileType : ScriptableObject
{
    [SerializeField] new public string name = "Tile";
    [SerializeField] public Sprite sprite = null;
    [SerializeField] public Color color = Color.white;
    [SerializeField] public bool walkable = true;
    [SerializeField] public float moveCost = 1.0f;
}
