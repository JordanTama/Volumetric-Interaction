using System;
using UnityEditor;
using UnityEngine;

namespace VolumetricInteraction.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(SettingsProfile))]
    public class SettingsProfileEditor : UnityEditor.Editor
    {
        private SerializedProperty _resolution;

        private void OnEnable()
        {
            _resolution = serializedObject.FindProperty("resolution");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Vector3Int res = _resolution.vector3IntValue;
            EditorGUILayout.LabelField((res.x * res.y * res.z) + " pixels");
        }
    }
}