using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to store adjacent tile requirements for a <see cref="TileTemplate"/>.
/// </summary>
[Serializable]
public class ConnectionsArray
{
    public bool[] data;

    public int rows;
    public int columns;

    public int Length => data.Length;
    
    public ConnectionsArray()
    {
        data = new bool[9];

        rows = 3;
        columns = 3;
    }

    public bool GetConnection(int row, int col) => GetConnection(row * rows + col);
    public bool GetConnection(int index) => data[index];
}
