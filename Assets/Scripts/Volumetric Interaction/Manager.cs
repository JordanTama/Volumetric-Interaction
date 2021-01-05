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
        public ComputeShader computeShader;
        
        private List<Volume> _volumes;
        private List<Source> _sources;

        private RenderTexture _texture;
        private ComputeBuffer _buffer;
        
        private static readonly int InteractionTexture = Shader.PropertyToID("interaction_texture");
        private static readonly int VolumeLocalToWorld = Shader.PropertyToID("volume_local_to_world");
        private static readonly int VolumeWorldToLocal = Shader.PropertyToID("volume_world_to_local");

        private const int MainKernelId = 0;
        private const string ComputeResultName = "result";


        public Volume FocusVolume => _volumes.Count > 0 ? _volumes[0] : null;
        

        #region Event Functions

        public void Initialize()
        {
            _volumes = new List<Volume>();
            _sources = new List<Source>();

            // InitializeBuffer();
            InitializeTexture();
        }

        public void InteractionUpdate(float delta)
        {
            ActorUpdate();
            UpdateTexture(delta);
        }

        #endregion
        
        
        #region Texture Generation

        public void InitializeTexture()
        {
            _texture = new RenderTexture(Resolution.x, Resolution.y, 0, RenderTextureFormat.ARGB32)
            { 
                dimension = TextureDimension.Tex3D,
                volumeDepth = Resolution.z,
                wrapMode = TextureWrapMode.Clamp,
                enableRandomWrite = true,
                filterMode = FilterMode.Point
            };
            
            _texture.Create();
        }

        private void InitializeBuffer()
        {
            _buffer = new ComputeBuffer(0, sizeof(float) * 4);
        }
        
        private void UpdateTexture(float delta)
        {
            if (!FocusVolume)
            {
                Debug.Log("No FocusVolume!");
                return;
            }

            if (FocusVolume.Count <= 0)
                return;
            
            // Set compute buffer data
            List<Seed> seeds = new List<Seed>();
            for (int i = 0; i < FocusVolume.Count; i++)
            {
                Source source = FocusVolume.GetSource(i);
                seeds.Add(new Seed(source.Position, source.Radius));
            }

            _buffer = new ComputeBuffer(seeds.Count, sizeof(float) * 4);
            _buffer.SetData(seeds);

            // Assign compute shader parameters
            computeShader.SetMatrix("volume_local_to_world", FocusVolume.transform.localToWorldMatrix);
            computeShader.SetInts("resolution", Resolution.x, Resolution.y, Resolution.z);
            computeShader.SetFloat("delta", delta);
            
            computeShader.SetTexture(MainKernelId, ComputeResultName, _texture);
            computeShader.SetBuffer(MainKernelId, "buffer", _buffer);
            
            // Dispatch compute shader
            computeShader.Dispatch(MainKernelId, Resolution.x / 8, Resolution.y / 8, Resolution.z / 8);

            // Assign global Shader variables
            Shader.SetGlobalTexture(InteractionTexture, _texture);
            
            Shader.SetGlobalMatrix(VolumeLocalToWorld, FocusVolume.transform.localToWorldMatrix);
            Shader.SetGlobalMatrix(VolumeWorldToLocal, FocusVolume.transform.worldToLocalMatrix);
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
        
        
        #region Data Structures

        private struct Seed
        {
            public Vector3 WorldPosition;
            public float Radius;

            public Seed(Vector3 worldPosition, float radius)
            {
                WorldPosition = worldPosition;
                Radius = radius;
            }
        }
        
        #endregion
    }
}