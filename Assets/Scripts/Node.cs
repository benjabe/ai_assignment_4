using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node<T>
{
    public T Data;      //
    public List<Edge<T>> Edges;   //list of all Nodes that are adjacent

    public Node()
    {
        Edges = new List<Edge<T>>();
    }
}


