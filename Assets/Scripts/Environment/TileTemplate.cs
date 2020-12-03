using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTemplate : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float unitsPerTile;
    [SerializeField] private ConnectionsArray connections = new ConnectionsArray();

    public bool Fits(bool[] occupied)
    {
        if (occupied.Length != connections.Length) return false;

        for (int i = 0; i < connections.Length; i++)
        {
            if (occupied[i] != connections.GetConnection(i))
                return false;
        }

        return true;
    }
}
