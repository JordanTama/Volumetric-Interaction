using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Environment.TilePainting
{
    /// <summary>
    /// Instantiates prefab tiles using a Texture2D guide.
    /// </summary>
    public class TilePainter : MonoBehaviour
    {
        [SerializeField] private Texture2D texture;
        [SerializeField] private float tileSize;
        [SerializeField] private TileTemplate[] tiles;

        public bool drawDebug;

        private List<GameObject> _instantiated;
    
    
        #region Generation Functions

        public void GenerateMap()
        {
            if (!texture)
            {
                Debug.Log("Texture missing!");
                return;
            }

            ClearMap();

            for (int index = 0; index < texture.width * texture.height; index++)
            {
                int col = index % texture.width;
                int row = index / texture.width;

                if (texture.GetPixel(col, row).a <= 0)
                    continue;

                // Create adjacent array
                bool[] adjacent = new bool[9];
                for (int rowReach = 1, count = 0; rowReach >= -1; rowReach--)
                {
                    for (int colReach = -1; colReach <= 1; colReach++, count++)
                    {
                        int r = row + rowReach;
                        int c = col + colReach;

                        if (r >= 0 && r < texture.height && c >= 0 && c < texture.width)
                            adjacent[count] = texture.GetPixel(c, r).a > 0;
                        else
                            adjacent[count] = false;
                    }
                }

                Vector3 position = transform.position;
                position -= (transform.right * texture.width * tileSize) / 2f;
                position -= (transform.forward * texture.height * tileSize) / 2f; 
                position += ((transform.right * tileSize) / 2f) * (1 + 2 * col);
                position += ((transform.forward * tileSize) / 2f) * (1 + 2 * row);

                // BUG: This needs fixing.
                List<int> matchList = new List<int>(); 
            
                for (int templateIndex = 0; templateIndex < tiles.Length; templateIndex++)
                {
                    matchList.Add(-1);    // The number of matching connections
                    matchList.Add(-1);    // The rotating of the template used
                
                    TileTemplate template = tiles[templateIndex];

                    for (int rotations = 0; rotations < 4; rotations++)
                    {
                        bool[] arr = RotateArray(adjacent, 3, rotations);

                        int rootIndex = templateIndex * 2;
                    
                        int matches = template.Fits(arr);

                        if (matches <= matchList[rootIndex]) continue;
                    
                        matchList[rootIndex] = matches;
                        matchList[rootIndex + 1] = rotations;
                    }
                }

                int maxIndex = -1;
            
                for (int templateIndex = 0; templateIndex < tiles.Length; templateIndex++)
                {
                    if (matchList[templateIndex * 2] > 0 && (maxIndex < 0 || matchList[templateIndex * 2] > matchList[maxIndex * 2]))
                    {
                        maxIndex = templateIndex;
                    }
                }

                if (maxIndex < 0 || maxIndex >= tiles.Length) continue;

                _instantiated.Add(InstantiateTemplate(tiles[maxIndex], position, matchList[maxIndex * 2 + 1]));
            }
        }

        public void ClearMap()
        {
            if (_instantiated is null)
            {
                _instantiated = new List<GameObject>();
                return;
            }
        
            foreach (GameObject go in _instantiated)
            {
                DestroyImmediate(go);
            }
        
            _instantiated.Clear();
        }
    
        private GameObject InstantiateTemplate(TileTemplate template, Vector3 position, int rotations)
        {
            Quaternion rotation = transform.rotation;
            rotation *= Quaternion.AngleAxis(rotations * -90f, transform.up);

            GameObject tile = Instantiate(template.Prefab, position, rotation, transform);
            tile.transform.localScale = (template.Prefab.transform.localScale / template.UnitsPerTile) * tileSize;

            return tile;
        }
    
        #endregion
    
    
        #region Utility Functions
    
        private static T[] RotateArray<T>(IReadOnlyList<T> array, int width, int clockwiseRotations)
        {
            // https://stackoverflow.com/questions/42519/how-do-you-rotate-a-two-dimensional-array
        
            clockwiseRotations = Mathf.Max(0, clockwiseRotations);
            T[] rotated = new T[width * width];
            for (int i = 0; i < rotated.Length; i++)
                rotated[i] = array[i];

            for (int i = 0; i < clockwiseRotations; i++)
                RotateArray(rotated, width);

            return rotated;
        }
    
        private static void RotateArray<T>(T[] array, int width)
        {
            T[] clone = new T[array.Length];
            for (int i = 0; i < clone.Length; i++)
                clone[i] = array[i];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    array[i * width + j] = clone[(width - j - 1) * width + i];
                }
            }
        }

        #endregion
    
    
        #region Debug
        
#if UNITY_EDITOR
    
        private void OnDrawGizmos()
        {
            if (drawDebug)
                DrawDebug();
        }

        private void DrawDebug()
        {
            if (!texture) return;

            Handles.color = Color.black;
            Handles.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            Vector2 extents = new Vector2(texture.width * tileSize / 2f, texture.height * tileSize / 2f);
        
            Vector3[] points = new Vector3[5];
            points[0] = new Vector3(-extents.x, 0, -extents.y);
            points[1] = new Vector3(-extents.x, 0, extents.y);
            points[2] = new Vector3(extents.x, 0, extents.y);
            points[3] = new Vector3(extents.x, 0, -extents.y);
            points[4] = new Vector3(-extents.x, 0, -extents.y);
        
            Handles.DrawAAPolyLine(Texture2D.whiteTexture, 3f, points);

            Handles.color = Color.white;
        
            Vector3 position = new Vector3(-extents.x + tileSize / 2f, 0, -extents.y + tileSize / 2f);
            Vector3 size = new Vector3(tileSize, 0, tileSize);
        
            for (int y = 0; y < texture.height; y++)
            {
                position.x = -extents.x + tileSize / 2f;
            
                for (int x = 0; x < texture.width; x++)
                {
                    if (texture.GetPixel(x, y).a > 0)
                        Handles.DrawWireCube(position, size);
                    position.x += tileSize;
                }

                position.z += tileSize;
            }
        }
    
#endif
        
        #endregion
    }
}
