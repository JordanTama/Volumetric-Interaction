using System;
using System.Collections.Generic;
using UnityEngine;

namespace VolumetricInteraction
{
    public class Manager : ScriptableObject
    {
        private readonly List<Volume> _volumes = new List<Volume>();
        private readonly List<Source> _sources = new List<Source>();


        #region Event Functions

        public void Initialize()
        {
            _volumes.Clear();
            _sources.Clear();
        }

        public void Update()
        {
            foreach (Volume vol in _volumes)
                vol.Clean();
            
            for (int i = _sources.Count - 1; i >= 0; i--)
            {
                foreach (Volume vol in _volumes)
                {
                    if (!vol.Bounds(_sources[i])) continue;

                    Move(_sources[i], vol);
                }
            }
        }

        #endregion
        
        
        #region Data Management

        public void Add(Volume volume)
        {
            if (!_volumes.Contains(volume))
                _volumes.Add(volume);
        }

        public void Remove(Volume volume)
        {
            if (!_volumes.Contains(volume)) return;
            
            volume.Clear();
            _volumes.Remove(volume);
        }

        public void Add(Source source)
        {
            if (_sources.Contains(source)) return;
            
            _sources.Add(source);
            source.Disassociate();
        }

        public void Remove(Source source)
        {
            if (_sources.Contains(source))
                _sources.Remove(source);
        }

        public void Move(Source source, Volume volume)
        {
            if (!_sources.Contains(source)) return;

            Remove(source);
            volume.Add(source);
            source.Associate(volume);
        }

        #endregion
        
        
        #region Debug
        
        public void DrawDebug()
        {
            foreach (Volume volume in _volumes)
                volume.DrawDebug();
            
            foreach (Source source in _sources)
                source.DrawDebug();
        }
        
        #endregion
    }
}