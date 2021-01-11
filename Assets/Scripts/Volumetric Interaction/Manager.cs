using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VolumetricInteraction
{
    public class Manager : ScriptableObject
    {
        public Vector3Int resolution;
        public FilterMode filterMode;
        public ComputeShader computeShader;
        
        private readonly List<Volume> _volumes = new List<Volume>();
        private readonly List<Source> _sources = new List<Source>();

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
            _volumes.Clear();
            _sources.Clear();;

            InitializeTexture();
        }

        public void InteractionUpdate(float delta)
        {
            // BUG: The order of Update and Tick determines whether the trail is drawn entering or exiting a volume...
            ActorUpdate();
            ActorTick();
            UpdateTexture(delta);
        }

        private void ActorTick()
        {
            foreach (Source source in _sources)
                source.OnTick();
            
            foreach (Volume volume in _volumes)
                volume.OnTick();
        }

        #endregion
        
        
        #region Texture Generation

        public void InitializeTexture()
        {
            _texture = new RenderTexture(resolution.x, resolution.y, 0, RenderTextureFormat.ARGB32)
            { 
                dimension = TextureDimension.Tex3D,
                volumeDepth = resolution.z,
                wrapMode = TextureWrapMode.Clamp,
                enableRandomWrite = true,
                filterMode = filterMode
            };
            
            _texture.Create();
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
            
            // Set compute buffer data - TODO: This can probably be optimized to not create a new buffer every tick...
            List<Seed> seeds = new List<Seed>();
            for (int i = 0; i < FocusVolume.Count; i++)
            {
                Source source = FocusVolume.GetSource(i);
                seeds.Add(new Seed(source.Position, source.PreviousPosition, source.Radius));
            }

            _buffer = new ComputeBuffer(seeds.Count, sizeof(float) * 7);
            _buffer.SetData(seeds);

            // Assign compute shader parameters
            computeShader.SetMatrix("volume_local_to_world", FocusVolume.transform.localToWorldMatrix);
            computeShader.SetInts("resolution", resolution.x, resolution.y, resolution.z);
            computeShader.SetFloat("delta", delta);
            
            computeShader.SetTexture(MainKernelId, ComputeResultName, _texture);
            computeShader.SetBuffer(MainKernelId, "buffer", _buffer);
            
            // Dispatch compute shader
            computeShader.Dispatch(MainKernelId, resolution.x / 8, resolution.y / 8, resolution.z / 8);

            // Assign global Shader variables
            Shader.SetGlobalTexture(InteractionTexture, _texture);
            
            Shader.SetGlobalMatrix(VolumeLocalToWorld, FocusVolume.transform.localToWorldMatrix);
            Shader.SetGlobalMatrix(VolumeWorldToLocal, FocusVolume.transform.worldToLocalMatrix);
            
            // Release the buffer
            _buffer.Release();
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
            public Vector3 PrevWorldPosition;
            public float Radius;

            public Seed(Vector3 worldPosition, Vector3 prevWorldPosition, float radius)
            {
                WorldPosition = worldPosition;
                PrevWorldPosition = prevWorldPosition;
                Radius = radius;
            }
        }
        
        #endregion
    }
}