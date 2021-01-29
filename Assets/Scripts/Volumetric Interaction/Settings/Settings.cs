using System;
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
        [SerializeField] private ComputeShader shader;
        [SerializeField] private bool drawGizmos;
        [SerializeField] private bool generateInEditor;
        
        // Singleton management
        private static Settings _instance;

        public static Settings Instance
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

        public static ComputeShader Shader
        {
            get
            {
                if (!Instance.shader)
                    Instance.shader = AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Shaders/Compute Shaders/VICompute.compute");

                return Instance.shader;
            }
        }

        
        #region Profile accessor properties
        
        public static SettingsProfile Profile => Instance.profile;
        public static Vector3Int Resolution => Profile.resolution;
        public static FilterMode FilterMode => Profile.filterMode;
        public static bool UseDecay => Profile.useDecay;
        public static float DecaySpeed => Profile.decaySpeed;
        public static bool UseBruteForce => Profile.useBruteForce;
        public static float TimeStep => Profile.timeStep;
        public static bool DrawGizmos => Instance.drawGizmos;
        public static bool GenerateInEditor => Instance.generateInEditor;

        #endregion
        

        public static void ApplyValues(SettingsProfile newProfile)
        {
            Profile.ApplyValues(newProfile);
            EditorUtility.SetDirty(Profile);
        }
    }
}