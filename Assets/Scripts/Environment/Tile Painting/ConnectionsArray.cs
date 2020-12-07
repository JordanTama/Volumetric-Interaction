using System;
using System.Linq;

namespace Environment.TilePainting
{
    /// <summary>
    /// Class used to store adjacent tile requirements for a <see cref="TileTemplate"/>.
    /// </summary>
    [Serializable]
    public class ConnectionsArray
    {
        public bool[] data;
        public int size;

    
        public int Length => data.Length;
        public int NumRequired => data.Count(connection => connection);
    
    
        public ConnectionsArray()
        {
            data = new bool[9];
            data[4] = true;

            size = 3;
        }
    

        public bool GetConnection(int row, int col) => GetConnection(row * size + col);

        public bool GetConnection(int index)
        {
            data[4] = true;
            return data[index];
        }
    }
}
