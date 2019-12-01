using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellPickup : MonoBehaviour
{
    public Tile Tile { get; protected set; }

    private void Start()
    {
        Tile = TileGraph.Instance.Tiles[
            Random.Range(0, TileGraph.Instance.Tiles.Count)
        ];

        transform.position = Tile.NodeTransform.position + Vector3.up * 0.2f;
    }
}
