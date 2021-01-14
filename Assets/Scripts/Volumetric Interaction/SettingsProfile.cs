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
    }
}