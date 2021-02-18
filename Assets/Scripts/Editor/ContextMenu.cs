using UnityEngine;
using UnityEditor;

/// <summary>
/// Can be extended to create custom context menus.
/// </summary>
public abstract class ContextMenu
{
    /// <summary>
    /// Creates a ScriptableObject of a given type.
    /// </summary>
    /// <typeparam name="T">Type of ScriptableObject.</typeparam>
    protected static void CreateScriptableObject<T>() where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += "/New " + typeof(T) + ".asset";

        ProjectWindowUtil.CreateAsset(asset, path);
    }

    /*
        Pattern:
        private const string Directory = "Assets/Create/ # Context Menu Directory # /";
        
        [MenuItem(Directory + # Menu Item Name #)]
        private static void # FunctionName # () => CreateScriptableObject< # ScriptableObject Type # >();
     */
}