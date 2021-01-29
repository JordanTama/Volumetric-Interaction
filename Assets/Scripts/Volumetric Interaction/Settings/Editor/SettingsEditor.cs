using System;
using UnityEditor;

namespace VolumetricInteraction.Editor
{
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty drawGizmos;
        private SerializedProperty generateInEditor;

        
        private void OnEnable()
        {
            drawGizmos = serializedObject.FindProperty("drawGizmos");
            generateInEditor = serializedObject.FindProperty("generateInEditor");
        }

        public override void OnInspectorGUI()
        {
            drawGizmos.boolValue = EditorGUILayout.Toggle("Draw Gizmos", drawGizmos.boolValue);
            generateInEditor.boolValue = EditorGUILayout.Toggle("Generate In Editor", generateInEditor.boolValue);

            serializedObject.ApplyModifiedProperties();
        }
    }
}