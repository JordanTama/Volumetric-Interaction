using UnityEditor;

namespace VolumetricInteraction.Editor
{
    public abstract class VolumetricInteractionMenu : ContextMenu
    {
        private const string Directory = "Assets/Create/Volumetric Interaction/";
        
        [MenuItem(Directory + "Settings Profile")]
        private static void NewSettingsPreset() => CreateScriptableObject<SettingsProfile>();
    }
}