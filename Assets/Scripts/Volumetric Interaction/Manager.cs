using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VolumetricInteraction
{
    // TODO: Draw out basic flow diagram for VI texture generation.
    public class Manager : ScriptableObject
    {
        public Vector3Int Resolution;
        
        private readonly List<Volume> _volumes = new List<Volume>();
        private readonly List<Source> _sources = new List<Source>();

        private RenderTexture _texture;
        
        private static readonly int InteractionTexture = Shader.PropertyToID("_InteractionTexture");


        public Volume FocusVolume => _volumes.Count > 0 ? _volumes[0] : null;
        

        #region Event Functions

        public void Initialize()
        {
            _volumes.Clear();
            _sources.Clear();

            _texture = new RenderTexture(Resolution.x, Resolution.y, 0)
            { 
                enableRandomWrite = true,
                dimension = TextureDimension.Tex3D,
                volumeDepth = Resolution.z
            };
        }

        public void InteractionUpdate(float delta)
        {
            ActorUpdate();
            UpdateTexture(delta);
        }

        #endregion
        
        
        #region Texture Generation

        private void UpdateTexture(float delta)
        {
            string debug = "";
            debug += "Manager -> updating texture...\n";
            debug += "delta = " + delta + "\n";
            Debug.Log(debug);
            
            // TODO: This may not need to be called everytime the texture changes...
            Shader.SetGlobalTexture(InteractionTexture, _texture);
        }
        
        #endregion
        
        
        #region Actor Management

        private void ActorUpdate()
        {
            foreach (Volume vol in _volumes)
                vol.Clean();

            for (int i = _sources.Count - 1; i >= 0; i--)
            {
                foreach (Volume vol in _volumes)
                {
                    if (!vol.Bounds(_sources[i])) continue;

                    Assign(_sources[i], vol);
                    break;
                }
            }
        }

        public void SetFocus(Volume volume)
        {
            if (!_volumes.Contains(volume)) return;

            _volumes.Remove(volume);
            _volumes.Insert(0, volume);
        }

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

        public void Assign(Source source, Volume volume)
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