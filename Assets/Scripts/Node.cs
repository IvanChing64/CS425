using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node: MonoBehaviour
{
    public bool walkable = true;
    

    public float gScore = float.MaxValue;
    public float hScore;
    public float fScore => gScore + hScore;

    public Node cameFrom;
    public List<Node> connections = new List<Node>();



}

