using System;
using UnityEditor;
using UnityEngine;

namespace VolumetricInteraction
{
    public class SettingsProfile : ScriptableObject
    {
        public Vector3Int resolution;
        public FilterMode filterMode;
        public ComputeShader computeShader;
        public int mainKernelId;
        public string computeResultName;
        public float decaySpeed;

        private void OnDestroy()
        {
            if (!Settings.Profile)
                Settings.CheckProfile();
        }

        public void ResetToDefault()
        {
            resolution = new Vector3Int(64, 64, 64);
            filterMode = FilterMode.Point;
            computeShader =
                AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Shaders/Compute Shaders/VICompute.compute");
            mainKernelId = 0;
            computeResultName = "result";
            decaySpeed = 0.5f;
        }
    }
}