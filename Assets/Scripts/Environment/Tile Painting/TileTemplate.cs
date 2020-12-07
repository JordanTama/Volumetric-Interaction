using UnityEngine;

namespace Environment.TilePainting
{
    public class TileTemplate : ScriptableObject
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private float unitsPerTile;
        [SerializeField] private ConnectionsArray connections = new ConnectionsArray();


        public GameObject Prefab => prefab;
        public float UnitsPerTile => unitsPerTile;
    
        private int NumRequired => connections.NumRequired;


        public int Fits(bool[] occupied)
        {
            int matching = 0;
            if (occupied.Length != connections.Length) return -1;
        
            for (int i = 0; i < connections.Length; i++)
            {
                if (connections.GetConnection(i) && occupied[i])
                    matching++;
            }

            return matching >= NumRequired ? matching : 0;
        }
    }
}
