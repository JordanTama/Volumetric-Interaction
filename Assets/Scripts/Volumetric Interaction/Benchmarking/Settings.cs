using System;
using UnityEngine;

namespace VolumetricInteraction.Benchmarking
{
    [Serializable]
    public struct Settings
    {
        public Parameter<Vector3Int> Resolution;
        public Parameter<int> SourceCount;

            
        public Settings(Parameter<Vector3Int> resolution, Parameter<int> sourceCount)
        {
            Resolution = resolution;
            SourceCount = sourceCount;
        }
    }
}