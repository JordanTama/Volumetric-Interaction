using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace VolumetricInteraction.Editor
{
    public class SettingsWindow : EditorWindow
    {
        private SettingsEditor _settingsEditor;
        
        private SettingsProfile _exposedProfile;
        private SettingsProfileEditor _profileEditor;

        private string _saveString = "New Profile";
        private SettingsProfile _loadTarget;

        private Vector2 _editorScroll;
        
        
        [MenuItem("Volumetric Interaction/Settings")]
        private static void ShowWindow()
        {
            var window = GetWindow<SettingsWindow>();
            window.titleContent = new GUIContent("VI Settings", EditorGUIUtility.IconContent("SettingsIcon").image);
            window.Show();
        }

        private void OnEnable()
        {
            _settingsEditor = (SettingsEditor) UnityEditor.Editor.CreateEditor(Settings.Instance);

            _exposedProfile = CloneInternal();
            _profileEditor = (SettingsProfileEditor) UnityEditor.Editor.CreateEditor(_exposedProfile);
        }

        private void OnGUI()
        {
            if (!_exposedProfile)
                OnEnable();
            
            DrawSaveLoad();
            
            DrawGlobalSettings();
            
            DrawDivider();

            DrawProfile();
            
            DrawDivider();
            
            DrawChangeButtons();
        }

        private void DrawSaveLoad()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            _loadTarget = (SettingsProfile) EditorGUILayout.ObjectField(_loadTarget, typeof(SettingsProfile), false);
            if (EditorGUI.EndChangeCheck())
                _saveString = _loadTarget ? _loadTarget.name : "New Profile";

            if (GUILayout.Button("Load") && _loadTarget)
            {
                _exposedProfile.ApplyValues(_loadTarget);
            }
            
            EditorGUILayout.EndHorizontal();

            
            EditorGUILayout.BeginHorizontal();

            _saveString = EditorGUILayout.TextField(_saveString);
            if (GUILayout.Button("Save"))
            {
                SettingsProfile newProfile = CreateInstance<SettingsProfile>();
                newProfile.ApplyValues(_exposedProfile);
                newProfile.name = _saveString;
                _loadTarget = null;

                AssetDatabase.CreateAsset(newProfile, "Assets/Resources/Volumetric Interaction/Profiles/" + newProfile.name + ".asset");
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawGlobalSettings()
        {
            _settingsEditor.OnInspectorGUI();
        }

        private void DrawProfile()
        {
            _editorScroll = EditorGUILayout.BeginScrollView(_editorScroll);
            _profileEditor.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawChangeButtons()
        {
            GUILayout.BeginHorizontal();
            
            GUI.enabled = _exposedProfile.CompareSettings(Settings.Profile);

            if (GUILayout.Button("Apply"))
            {
                Settings.ApplyValues(_exposedProfile);
                OnEnable();
            }
            if (GUILayout.Button("Revert"))
            {
                OnEnable();
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
        }

        private static SettingsProfile CloneInternal()
        {
            SettingsProfile exposed = Instantiate(Settings.Profile);
            exposed.name = "exposedProfile";
            return exposed;
        }

        private static void DrawDivider()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }
}