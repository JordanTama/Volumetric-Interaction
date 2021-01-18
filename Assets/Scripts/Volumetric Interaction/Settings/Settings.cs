using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VolumetricInteraction
{
    [Serializable]
    public class Settings : ScriptableObject
    {
        private const string SettingsPath = "Assets/Resources/Volumetric Interaction/Settings/";
        private const string InstancePath = SettingsPath + "SettingsInstance.asset";
        private const string ProfilePath = SettingsPath + "InternalProfile.asset";
        
        // Serialized member variables
        [SerializeField] private SettingsProfile profile;
        
        // Singleton management
        private static Settings _instance;

        private static Settings Instance
        {
            get
            {
                if (!_instance)
                    _instance = AssetDatabase.LoadAssetAtPath<Settings>(InstancePath);

                if (_instance)
                    return _instance;
                
                Settings instance = CreateInstance<Settings>();
                instance.profile = AssetDatabase.LoadAssetAtPath<SettingsProfile>(ProfilePath);
                
                AssetDatabase.CreateAsset(instance, InstancePath);

                return _instance = instance;
            }
        }

        
        #region Preset accessor properties
        
        public static SettingsProfile Profile => Instance.profile;
        
        public static Vector3Int Resolution => Profile.resolution;

        public static FilterMode FilterMode => Profile.filterMode;

        public static ComputeShader ComputeShader => Profile.computeShader;

        public static int MainKernelId => Profile.mainKernelId;

        public static string ComputeResultName => Profile.computeResultName;

        public static float DecaySpeed => Profile.decaySpeed;
        
        #endregion
        

        public static void ApplyValues(SettingsProfile newProfile)
        {
            Profile.ApplyValues(newProfile);
            EditorUtility.SetDirty(Profile);
        }

        private void ResetProfile()
        {
            profile.ResetToDefault();
            profile.name = "internalProfile";
        }
    }
}