using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Developer: Andrew Shelton
//Developed with help from Game Dev Garnet on youtube

public class Node: MonoBehaviour
{
    public bool walkable = true;
    

    public float gScore = float.MaxValue;
    public float hScore;
    public float fScore => gScore + hScore;

    public Node cameFrom;
    public List<Node> connections = new List<Node>();



}

