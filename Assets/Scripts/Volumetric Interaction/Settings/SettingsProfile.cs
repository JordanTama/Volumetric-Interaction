using System;
using UnityEngine;

namespace VolumetricInteraction
{
    [Serializable]
    public class SettingsProfile : ScriptableObject
    {
        public Vector3Int resolution;
        public FilterMode filterMode;
        public bool useDecay;
        public float decaySpeed;
        public bool useBruteForce;
        public float timeStep;

        public void ResetToDefault()
        {
            resolution = new Vector3Int(64, 64, 64);
            filterMode = FilterMode.Point;
            useDecay = true;
            decaySpeed = 0.5f;
            useBruteForce = false;
            timeStep = 0.05f;
        }

        public bool CompareSettings(SettingsProfile other)
        {
            if (resolution != other.resolution)
                return true;

            if (filterMode != other.filterMode)
                return true;

            if (useDecay != other.useDecay)
                return true;

            if (!Mathf.Approximately(decaySpeed, other.decaySpeed))
                return true;

            if (useBruteForce != other.useBruteForce)
                return true;

            if (!Mathf.Approximately(timeStep, other.timeStep))
                return true;

            return false;
        }

        public void ApplyValues(SettingsProfile other)
        {
            resolution = other.resolution;
            filterMode = other.filterMode;
            useDecay = other.useDecay;
            decaySpeed = other.decaySpeed;
            useBruteForce = other.useBruteForce;
            timeStep = other.timeStep;
        }
    }
}