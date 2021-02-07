using System;
using UnityEditor;
using UnityEngine;

namespace VolumetricInteraction.Editor
{
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty _drawGizmos;
        private SerializedProperty _generateInEditor;
        private SerializedProperty _steps;
        private SerializedProperty _debugSteps;
        private SerializedProperty _shader;

        
        private void OnEnable()
        {
            _drawGizmos = serializedObject.FindProperty("drawGizmos");
            _generateInEditor = serializedObject.FindProperty("generateInEditor");
            _steps = serializedObject.FindProperty("steps");
            _debugSteps = serializedObject.FindProperty("debugSteps");
            _shader = serializedObject.FindProperty("shader");
        }

        public override void OnInspectorGUI()
        {
            _shader.objectReferenceValue =
                EditorGUILayout.ObjectField("Shader", _shader.objectReferenceValue, typeof(ComputeShader), false);
            
            _drawGizmos.boolValue = EditorGUILayout.Toggle("Draw Gizmos", _drawGizmos.boolValue);
            _generateInEditor.boolValue = EditorGUILayout.Toggle("Generate In Editor", _generateInEditor.boolValue);

            int floodIterations = (int) Mathf.Log(
                Mathf.Max(Settings.Resolution.x, Settings.Resolution.y, Settings.Resolution.z)
                , 2);
            
            int maxSteps = 1 + floodIterations;

            string[] names = new string[maxSteps + 1];
            
            names[0] = "Seeding Pass";
            names[names.Length - 1] = "Conversion Pass";
            
            for (int i = 0; i < floodIterations; i++)
                names[i + 1] = "JFA: step size = " + Mathf.Pow(2, floodIterations - i - 1);

            if (!Settings.UseBruteForce)
            {
                EditorGUILayout.BeginHorizontal();

                _debugSteps.boolValue = EditorGUILayout.Toggle("Debug Steps", _debugSteps.boolValue);
                EditorGUILayout.BeginVertical();
                GUI.enabled = _debugSteps.boolValue;
                int sliderValue = EditorGUILayout.IntSlider(_steps.intValue, 0, maxSteps);
                GUI.enabled = true;

                _steps.intValue = _debugSteps.boolValue
                    ? sliderValue
                    : maxSteps;
                
                if (_debugSteps.boolValue)
                    EditorGUILayout.LabelField(names[_steps.intValue]);
                
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}