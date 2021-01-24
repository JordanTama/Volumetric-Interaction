using UnityEditor;

namespace VolumetricInteraction.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(SettingsProfile))]
    public class SettingsProfileEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}