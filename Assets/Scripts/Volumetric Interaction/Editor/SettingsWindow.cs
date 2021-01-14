using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace VolumetricInteraction.Editor
{
    [SuppressMessage("ReSharper", "AssignmentInConditionalExpression")]
    public class SettingsWindow : EditorWindow
    {
        private Vector2 _scrollPos;
        
        private string saveName = "New Settings Profile";
        
        private const string Path = "Assets/Resources/Volumetric Interaction/Profiles/";
        
        private bool _advancedFoldout = false;

        private UnityEditor.Editor profileEditor;

        private GUIStyle _headerStyle;

        // Window Functions
        
        [MenuItem("Window/Volumetric Interaction/Settings")]
        private static void ShowWindow()
        {
            var window = GetWindow<SettingsWindow>();
            window.titleContent = new GUIContent("VI Settings", EditorGUIUtility.IconContent("SettingsIcon").image);
            window.Show();
        }

        private void OnEnable()
        {
            _headerStyle = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = Color.white
                }
            };
        }

        private void OnGUI()
        {
            DrawProfileField();

            DrawProfileHeader(_headerStyle);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            profileEditor.DrawDefaultInspector();
            EditorGUILayout.EndScrollView();

            DrawSaveField();
            
            DrawAdvancedSettings();
        }

        
        // Element functions
        
        private void DrawProfileField()
        {
            SettingsProfile profileField =
                (SettingsProfile) EditorGUILayout.ObjectField("Profile", Settings.Profile, typeof(SettingsProfile),
                    false);

            if (!profileField)
            {
                profileField = CloneProfile(Settings.DefaultProfile, "New Settings Profile");
            }

            if (profileField == Settings.Profile)
                return;
            
            Settings.SetProfile(profileField);
            
            profileEditor = UnityEditor.Editor.CreateEditor(Settings.Profile);
            saveName = Settings.Profile.name;
        }

        private void DrawSaveField()
        {
            EditorGUILayout.Space();
            
            GUILayout.BeginHorizontal();
            saveName = EditorGUILayout.TextField(saveName);

            DrawRenameButton(Settings.Profile, saveName);

            DrawSaveButton(saveName);
            
            GUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
        }

        private void DrawAdvancedSettings()
        {
            if (!(_advancedFoldout = EditorGUILayout.Foldout(_advancedFoldout, "Advanced Settings")))
                return;
            
            EditorGUI.indentLevel++;
            
            // Draw default profile field
            SettingsProfile defaultProfile = (SettingsProfile) EditorGUILayout.ObjectField("Default Profile",
                Settings.DefaultProfile, typeof(SettingsProfile), false);
            
            Settings.SetDefaultProfile(defaultProfile);
            
            EditorGUI.indentLevel--;
        }

        
        // Static methods
        
        private static SettingsProfile CloneProfile(SettingsProfile profile, string name)
        {
            SettingsProfile clone = Instantiate(profile);
            clone.name = name;
            return clone;
        }

        private static void DrawProfileHeader(GUIStyle style)
        {
            EditorGUILayout.Space();
            
            Rect rect = EditorGUILayout.GetControlRect(false, style.fontSize * 1.3f);
            EditorGUI.LabelField(rect, Settings.Profile.name, style);
            
            EditorGUILayout.Space();
        }

        private static SettingsProfile Save(SettingsProfile profile, string name)
        {
            SettingsProfile newProfile = Instantiate(profile);
            profile = newProfile;
            
            string fileName = profile.name = name;
            const string extension = ".asset";

            string GetNumber(int n) => n > 0 ? " " + n : "";
            
            int number = 0;
            while (AssetDatabase.LoadAssetAtPath<SettingsProfile>(Path + fileName + GetNumber(number) + extension))
                number++;

            AssetDatabase.CreateAsset(profile, Path + fileName + GetNumber(number) + extension);

            return profile;
        }

        private static void DrawRenameButton(SettingsProfile profile, string name)
        {
            bool exists = AssetDatabase.Contains(profile);

            if (GUILayout.Button("Rename"))
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(profile), name);
            }
        }

        private static void DrawSaveButton(string name)
        {
            if (GUILayout.Button("Save"))
                Settings.SetProfile(Save(Settings.Profile, name));
        }
    }
}