using UnityEditor;

namespace VolumetricInteraction.Editor
{
    [CustomEditor(typeof(SettingsProfile))]
    public class SettingsProfileEditor : UnityEditor.Editor
    {
        public bool hasChanged;
        
        private bool _defaultInspectorFoldout;
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            hasChanged = EditorGUI.EndChangeCheck();
        }
    }
}