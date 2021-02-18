using System;
using UnityEditor;
using UnityEngine;

namespace VolumetricInteraction.Editor
{
    [CustomEditor(typeof(Visualiser))]
    public class VisualiserEditor : UnityEditor.Editor
    {
        private SerializedProperty _fileName;

        private const string Directory = "Assets/Resources/Screen Captures/";
        private const string Extension = ".png";

        public void OnEnable()
        {
            _fileName = serializedObject.FindProperty("fileName");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!GUILayout.Button("Capture Screen")) 
                return;
            
            ScreenCapture.CaptureScreenshot(Directory + _fileName.stringValue + Extension);
            AssetDatabase.Refresh();
        }
    }
}