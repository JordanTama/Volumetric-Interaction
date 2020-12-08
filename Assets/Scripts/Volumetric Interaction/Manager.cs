using System;
using System.Collections.Generic;
using UnityEngine;

namespace VolumetricInteraction
{
    public class Manager : ScriptableObject
    {
        private readonly List<Volume> _volumes = new List<Volume>();
        private readonly List<Source> _sources = new List<Source>();

        public void DrawDebug()
        {
            foreach (Volume volume in _volumes)
                volume.DrawDebug();
            
            foreach (Source source in _sources)
                source.DrawDebug();
        }
    }
}