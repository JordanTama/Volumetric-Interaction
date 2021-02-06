using System;
using UnityEditor;
using UnityEngine;

namespace VolumetricInteraction.Editor
{
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty drawGizmos;
        private SerializedProperty generateInEditor;
        private SerializedProperty steps;
        private SerializedProperty debugSteps;
        private SerializedProperty shader;

        
        private void OnEnable()
        {
            drawGizmos = serializedObject.FindProperty("drawGizmos");
            generateInEditor = serializedObject.FindProperty("generateInEditor");
            steps = serializedObject.FindProperty("steps");
            debugSteps = serializedObject.FindProperty("debugSteps");
            shader = serializedObject.FindProperty("shader");
        }

        public override void OnInspectorGUI()
        {
            shader.objectReferenceValue =
                EditorGUILayout.ObjectField("Shader", shader.objectReferenceValue, typeof(ComputeShader), false);
            
            drawGizmos.boolValue = EditorGUILayout.Toggle("Draw Gizmos", drawGizmos.boolValue);
            generateInEditor.boolValue = EditorGUILayout.Toggle("Generate In Editor", generateInEditor.boolValue);

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

                debugSteps.boolValue = EditorGUILayout.Toggle("Debug Steps", debugSteps.boolValue);
                EditorGUILayout.BeginVertical();
                GUI.enabled = debugSteps.boolValue;
                int sliderValue = EditorGUILayout.IntSlider(steps.intValue, 0, maxSteps);
                GUI.enabled = true;

                steps.intValue = debugSteps.boolValue
                    ? sliderValue
                    : maxSteps;
                
                if (debugSteps.boolValue)
                    EditorGUILayout.LabelField(names[steps.intValue]);
                
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}