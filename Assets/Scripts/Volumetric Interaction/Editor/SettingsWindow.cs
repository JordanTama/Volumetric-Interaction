using UnityEditor;
using UnityEngine;

namespace VolumetricInteraction.Editor
{
    public class SettingsWindow : EditorWindow
    {
        private Vector2 _scrollPos;

        private const string Path = "Assets/Resources/Volumetric Interaction/";
        
        [MenuItem("Window/Volumetric Interaction/Settings")]
        private static void ShowWindow()
        {
            var window = GetWindow<SettingsWindow>();
            window.titleContent = new GUIContent("VI Settings", EditorGUIUtility.IconContent("SettingsIcon").image);
            window.Show();
        }

        private void OnGUI()
        {
            Settings.SetProfile((SettingsProfile) EditorGUILayout.ObjectField("Settings Profile", Settings.Profile,
                typeof(SettingsProfile), false));

            if (!DrawCreateButton())
                return;

            DrawProfileInspector();
        }

        private bool DrawCreateButton()
        {
            if (Settings.Profile)
                return true;
            
            if (!GUILayout.Button("New Settings Profile"))
                return false;

            SettingsProfile profile = CreateInstance<SettingsProfile>();

            const string fileName = "New Settings Profile";
            const string extension = ".asset";

            string GetNumber(int n) => n > 0 ? " " + n : "";
            
            int number = 0;
            while (AssetDatabase.LoadAssetAtPath<SettingsProfile>(Path + fileName + GetNumber(number) + extension))
                number++;

            AssetDatabase.CreateAsset(profile, Path + fileName + GetNumber(number) + extension);
            
            EditorGUIUtility.PingObject(profile);
            Selection.activeObject = profile;
            
            Settings.SetProfile(profile);

            return true;
        }

        private void DrawProfileInspector()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            UnityEditor.Editor.CreateEditor(Settings.Profile).OnInspectorGUI();
            
            EditorGUILayout.EndScrollView();
        }
    }
}