using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

    public void GenerateMap()
    {
        if (!texture)
        {
            Debug.Log("Texture missing!");
            return;
        }

        ClearMap();
        
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                
            }
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
            Destroy(go);
        }
        
        _instantiated.Clear();
    }

    private static T[] RotateArray<T>(T[] array, int width, int height)
    {
        throw new NotImplementedException();
    }

    
    #region Debug
    
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
    
    #endregion
}
