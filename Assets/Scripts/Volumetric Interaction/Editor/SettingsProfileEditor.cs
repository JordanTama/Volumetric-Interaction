using UnityEditor;

namespace VolumetricInteraction.Editor
{
    [CustomEditor(typeof(SettingsProfile))]
    public class SettingsProfileEditor : UnityEditor.Editor
    {
        private bool _defaultInspectorFoldout;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}