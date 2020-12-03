using UnityEngine;
using UnityEditor;

namespace Environment
{
    public abstract class EnvironmentMenu : ContextMenu
    {
        private const string Directory = "Assets/Create/Environment/";
        
        [MenuItem(Directory + "Tile Template")]
        private static void NewTileTemplate() => CreateScriptableObject<TileTemplate>();
    }
}