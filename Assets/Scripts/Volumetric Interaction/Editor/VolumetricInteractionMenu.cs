using UnityEditor;

namespace VolumetricInteraction.Editor
{
    public abstract class VolumetricInteractionMenu : ContextMenu
    {
        private const string Directory = "Assets/Create/Volumetric Interaction/";
        
        [MenuItem(Directory + "Manager")]
        private static void NewTileTemplate() => CreateScriptableObject<Manager>();
    }
}