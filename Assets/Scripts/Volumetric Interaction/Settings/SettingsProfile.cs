using System;
using UnityEditor;
using UnityEngine;

namespace VolumetricInteraction
{
    [Serializable]
    public class SettingsProfile : ScriptableObject
    {
        public Vector3Int resolution;
        public FilterMode filterMode;
        public ComputeShader computeShader;
        public bool useBruteForce;
        public float decaySpeed;

        public void ResetToDefault()
        {
            resolution = new Vector3Int(64, 64, 64);
            filterMode = FilterMode.Point;
            computeShader =
                AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Shaders/Compute Shaders/VICompute.compute");
            decaySpeed = 0.5f;
        }

        public bool CompareSettings(SettingsProfile other)
        {
            if (resolution != other.resolution)
                return true;

            if (filterMode != other.filterMode)
                return true;

            if (computeShader != other.computeShader)
                return true;

            if (useBruteForce != other.useBruteForce)
                return true;

            if (!Mathf.Approximately(decaySpeed, other.decaySpeed))
                return true;

            return false;
        }

        public void ApplyValues(SettingsProfile other)
        {
            resolution = other.resolution;
            filterMode = other.filterMode;
            computeShader = other.computeShader;
            useBruteForce = other.useBruteForce;
            decaySpeed = other.decaySpeed;
        }
    }
}