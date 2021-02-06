using System;
using UnityEditor;
using UnityEngine;

namespace VolumetricInteraction
{
    [Serializable]
    public class Settings : ScriptableObject
    {
        private const string SettingsPath = "Volumetric Interaction/Settings/";
        private const string InstancePath = SettingsPath + "SettingsInstance";
        private const string ProfilePath = SettingsPath + "InternalProfile";
        
        // Serialized member variables
        [SerializeField] private SettingsProfile profile;
        [SerializeField] private ComputeShader shader;
        [SerializeField] private bool drawGizmos;
        [SerializeField] private bool generateInEditor;
        [SerializeField] private int steps;
        [SerializeField] private bool debugSteps;
        
        // Singleton management
        private static Settings _instance;

        public static Settings Instance
        {
            get
            {
                if (!_instance)
                    _instance = Resources.Load<Settings>(SettingsPath);

                if (_instance)
                    return _instance;
                
                Settings instance = CreateInstance<Settings>();
                instance.profile = Resources.Load<SettingsProfile>(ProfilePath);

#if UNITY_EDITOR
                AssetDatabase.CreateAsset(instance, "Assets/Resources/" + InstancePath);
#endif

                return _instance = instance;
            }
        }

        public static ComputeShader Shader => Instance.shader;

        
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
        public static int Steps => Instance.steps;
        public static bool DebugSteps => Instance.debugSteps;

        #endregion
        

        public static void ApplyValues(SettingsProfile newProfile)
        {
            Profile.ApplyValues(newProfile);
            
#if UNITY_EDITOR
            EditorUtility.SetDirty(Profile);
#endif
            
            Core.Initialize();
        }
    }
}