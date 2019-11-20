using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge<T>
{
    public float Cost { get; protected set; } //cost to traverse edge = cost to enter tile

    public Node<T> Node { get; protected set; }

    public Edge(float cost, Node<T> node)
    {
        Cost = cost;
        Node = node;
    }
}
