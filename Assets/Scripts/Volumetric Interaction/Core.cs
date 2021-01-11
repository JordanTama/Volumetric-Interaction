using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VolumetricInteraction
{
    public static class Core
    {
        private static readonly List<Volume> _volumes = new List<Volume>();
        private static readonly List<Source> _sources = new List<Source>();

        private static RenderTexture _texture;
        private static ComputeBuffer _buffer;
        
        private static readonly int InteractionTexture = Shader.PropertyToID("interaction_texture");
        private static readonly int VolumeLocalToWorld = Shader.PropertyToID("volume_local_to_world");
        private static readonly int VolumeWorldToLocal = Shader.PropertyToID("volume_world_to_local");
        
        public static Volume FocusVolume => _volumes.Count > 0 ? _volumes[0] : null;
        

        #region Event Functions

        public static void Initialize()
        {
            Volume[] volumes = _volumes.ToArray();
            foreach (Volume volume in volumes)
                volume.Clear();
            
            Source[] sources = _sources.ToArray();
            
            _volumes.Clear();
            _sources.Clear();

            foreach (Volume volume in volumes)
            {
                if (volume)
                    Add(volume);
            }

            foreach (Source source in sources)
            {
                if (source)
                    Add(source);
            }

            InitializeTexture();
        }

        public static void InteractionUpdate(float delta)
        {
            ActorTick();
            ActorUpdate();
            UpdateTexture(delta);
        }

        private static void ActorTick()
        {
            foreach (Source source in _sources)
                source.OnTick();
            
            foreach (Volume volume in _volumes)
                volume.OnTick();
        }

        #endregion
        
        
        #region Texture Generation

        private static void InitializeTexture()
        {
            _texture = new RenderTexture(Settings.Resolution.x, Settings.Resolution.y, 0, RenderTextureFormat.ARGB32)
            { 
                dimension = TextureDimension.Tex3D,
                volumeDepth = Settings.Resolution.z,
                wrapMode = TextureWrapMode.Clamp,
                enableRandomWrite = true,
                filterMode = Settings.FilterMode
            };
            
            _texture.Create();
        }
        
        private static void UpdateTexture(float delta)
        {
            if (!FocusVolume)
            {
                Debug.Log("No FocusVolume!");
                return;
            }

            if (FocusVolume.Count <= 0)
                return;
            
            List<Seed> seeds = new List<Seed>();
            for (int i = 0; i < FocusVolume.Count; i++)
            {
                Source source = FocusVolume.GetSource(i);
                seeds.Add(new Seed(source.Position, source.PreviousPosition, source.Radius));
            }

            _buffer = new ComputeBuffer(seeds.Count, sizeof(float) * 7);
            _buffer.SetData(seeds);

            // Assign compute shader parameters
            Settings.ComputeShader.SetMatrix("volume_local_to_world", FocusVolume.transform.localToWorldMatrix);
            Settings.ComputeShader.SetInts("resolution", _texture.width, _texture.height, _texture.volumeDepth);
            Settings.ComputeShader.SetFloat("delta", delta);
            
            Settings.ComputeShader.SetTexture(Settings.MainKernelId, Settings.ComputeResultName, _texture);
            Settings.ComputeShader.SetBuffer(Settings.MainKernelId, "buffer", _buffer);
            
            // Dispatch compute shader
            Settings.ComputeShader.Dispatch(Settings.MainKernelId, _texture.width / 8, _texture.height / 8, _texture.volumeDepth / 8);

            // Assign global Shader variables
            Shader.SetGlobalTexture(InteractionTexture, _texture);
            
            Shader.SetGlobalMatrix(VolumeLocalToWorld, FocusVolume.transform.localToWorldMatrix);
            Shader.SetGlobalMatrix(VolumeWorldToLocal, FocusVolume.transform.worldToLocalMatrix);
            
            // Release the buffer
            _buffer.Release();
        }
        
        #endregion
        
        
        #region Actor Management

        private static void ActorUpdate()
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

        public static void SetFocus(Volume volume)
        {
            if (!_volumes.Contains(volume)) return;

            _volumes.Remove(volume);
            _volumes.Insert(0, volume);
        }

        public static void Add(Volume volume)
        {
            if (!_volumes.Contains(volume))
                _volumes.Add(volume);
        }

        public static void Remove(Volume volume)
        {
            if (!_volumes.Contains(volume)) return;
            
            volume.Clear();
            _volumes.Remove(volume);
        }

        public static void Add(Source source)
        {
            if (_sources.Contains(source)) return;
            
            _sources.Add(source);
            source.Disassociate();
        }

        public static void Remove(Source source)
        {
            if (_sources.Contains(source))
                _sources.Remove(source);
        }

        public static void Assign(Source source, Volume volume)
        {
            if (!_sources.Contains(source)) return;

            Remove(source);
            volume.Add(source);
            source.Associate(volume);
        }

        #endregion
        
        
        #region Debug
        
        public static void DrawDebug()
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