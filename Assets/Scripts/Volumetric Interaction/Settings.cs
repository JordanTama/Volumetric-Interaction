﻿using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VolumetricInteraction
{
    public class Settings : ScriptableObject
    {
        private const string Path = "Assets/Resources/Volumetric Interaction/SettingsInstance.asset";
        
        // Serialized member variables
        [SerializeField] private SettingsProfile profile;
        
        // Singleton management
        private static Settings _instance;

        private static Settings Instance
        {
            get
            {
                if (!_instance)
                    _instance = AssetDatabase.LoadAssetAtPath<Settings>(Path);

                if (_instance)
                    return _instance;
                
                Settings instance = CreateInstance<Settings>();
                AssetDatabase.CreateAsset(instance, Path);

                return _instance = instance;
            }
        }

        // Preset accessor properties
        public static SettingsProfile Profile => Instance.profile;
        
        public static Vector3Int Resolution => Profile.resolution;

        public static FilterMode FilterMode => Profile.filterMode;

        public static ComputeShader ComputeShader => Profile.computeShader;

        public static int MainKernelId => Profile.mainKernelId;

        public static string ComputeResultName => Profile.computeResultName;

        public static float DecaySpeed => Profile.decaySpeed;
        

        // Methods
        public static void CheckProfile()
        {
            if (!Profile)
                Instance.ResetProfile();
        }
        
        
        private void OnEnable()
        {
            if (!profile)
                ResetProfile();
        }

        private void ResetProfile()
        {
            SettingsProfile newProfile = CreateInstance<SettingsProfile>();
            newProfile.ResetToDefault();
            newProfile.name = "internalProfile";
            
            profile = newProfile;
        }
    }
}