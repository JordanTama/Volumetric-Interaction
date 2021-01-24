using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VolumetricInteraction
{
    // BUG: Source doesn't draw (but IS managed) when on the POSITIVE bounds of the volume.
    // BUG: Alpha channel is 0-1, need to find another way to transfer radius information.
    // TODO: Create proper conversion functions for coordinate spaces in compute shader.
    // TODO: Implement texture blending.
    // TODO: Implement a way of stepping through the texture generation gradually to visualise the process.
    public static class Core
    {
        private static readonly List<Volume> Volumes = new List<Volume>();
        private static readonly List<Source> Sources = new List<Source>();

        private static RenderTexture _texture;
        private static ComputeBuffer _buffer;
        
        private static readonly int InteractionTexture = Shader.PropertyToID("interaction_texture");
        private static readonly int VolumeLocalToWorld = Shader.PropertyToID("volume_local_to_world");
        private static readonly int VolumeWorldToLocal = Shader.PropertyToID("volume_world_to_local");
        
        public static Volume FocusVolume => Volumes.Count > 0 ? Volumes[0] : null;
        

        #region Event Functions

        public static void Initialize()
        {
            // if (!Settings.Profile)
            //     Settings.ReassignProfile();
            
            Volume[] volumes = Volumes.ToArray();
            foreach (Volume volume in volumes)
                volume.Clear();
            
            Source[] sources = Sources.ToArray();
            
            Volumes.Clear();
            Sources.Clear();

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
            foreach (Source source in Sources)
                source.OnTick();
            
            foreach (Volume volume in Volumes)
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
            // STEP 1. Ensure the required components are present.
            if (!FocusVolume || Settings.ComputeShader is null)
            {
                Debug.Log("No FocusVolume or ComputeShader!");
                return;
            }

            // TODO: Decay even if no sources are present in the focus volume...
            if (FocusVolume.Count <= 0)
                return;
            
            // STEP 2. Construct compute buffer.
            float multiplier = 0;
            List<Seed> seeds = new List<Seed>();
            for (int i = 0; i < FocusVolume.Count; i++)
            {
                Source source = FocusVolume.GetSource(i);
                seeds.Add(new Seed(source.Position, source.PreviousPosition, source.Radius));

                multiplier = Mathf.Max(source.Radius, multiplier);
            }

            _buffer = new ComputeBuffer(seeds.Count, sizeof(float) * 7);
            _buffer.SetData(seeds);
            
            // STEP 3. Update global compute shader variables.
            Settings.ComputeShader.SetMatrix("volume_local_to_world", FocusVolume.transform.localToWorldMatrix);
            Settings.ComputeShader.SetMatrix("volume_world_to_local", FocusVolume.transform.worldToLocalMatrix);
            Settings.ComputeShader.SetInts("resolution", _texture.width, _texture.height, _texture.volumeDepth);
            Settings.ComputeShader.SetFloat("delta", delta);
            Settings.ComputeShader.SetFloat("decay_speed", Settings.DecaySpeed);
            Settings.ComputeShader.SetFloat("radius_multiplier", multiplier);
            
            // STEP 4. Run main texture generation method.
            if (Settings.UseBruteForce)
                BruteUpdateTexture();
            else
                FloodUpdateTexture();
            
            // STEP 5. Assign global shader variables.
            Shader.SetGlobalTexture(InteractionTexture, _texture);
            
            Shader.SetGlobalMatrix(VolumeLocalToWorld, FocusVolume.transform.localToWorldMatrix);
            Shader.SetGlobalMatrix(VolumeWorldToLocal, FocusVolume.transform.worldToLocalMatrix);
            
            
            // STEP 6. Release the buffer.
            _buffer.Release();
        }
        
        private static void BruteUpdateTexture()
        {
            Settings.ComputeShader.SetTexture(0, "result", _texture);
            Settings.ComputeShader.SetBuffer(0, "buffer", _buffer);
            
            // Dispatch compute shader
            Settings.ComputeShader.Dispatch(0, _texture.width / 8, _texture.height / 8, _texture.volumeDepth / 8);
        }

        private static void FloodUpdateTexture()
        {
            // STEP 1. Sentinel pass.
            Settings.ComputeShader.SetTexture(3, "result", _texture);
            Settings.ComputeShader.SetBuffer(3, "buffer", _buffer);
            
            Settings.ComputeShader.Dispatch(3, _texture.width / 8, _texture.height / 8, _texture.volumeDepth / 8);

            // STEP 2. Seeding pass.
            Settings.ComputeShader.SetTexture(1, "result", _texture);
            Settings.ComputeShader.SetBuffer(1, "buffer", _buffer);

            Settings.ComputeShader.Dispatch(1, _buffer.count, 1, 1);
            
            // STEP 3. Jump Flooding Algorithm pass.
            Settings.ComputeShader.SetTexture(2, "result", _texture);
            Settings.ComputeShader.SetBuffer(2, "buffer", _buffer);

            Vector3Int step = new Vector3Int(_texture.width, _texture.height, _texture.volumeDepth);
            while (step.x > 1 || step.y > 1 || step.z >  1)
            {
                step.x = Mathf.Max(1, step.x / 2);
                step.y = Mathf.Max(1, step.y / 2);
                step.z = Mathf.Max(1, step.z / 2);
                
                Settings.ComputeShader.SetInts("step_size", step.x, step.y, step.z);

                Vector3Int threadGroupSize =
                    new Vector3Int(_texture.width / 8, _texture.height / 8, _texture.volumeDepth / 8);
                
                Settings.ComputeShader.Dispatch(2, threadGroupSize.x, threadGroupSize.y, threadGroupSize.z);
            }
            
            // STEP 4. Conversion pass.
            Settings.ComputeShader.SetTexture(4, "result", _texture);
            Settings.ComputeShader.SetBuffer(4, "buffer", _buffer);

            Settings.ComputeShader.Dispatch(4, _texture.width / 8, _texture.height / 8, _texture.volumeDepth / 8);
        }
        
        #endregion
        
        
        #region Actor Management

        private static void ActorUpdate()
        {
            foreach (Volume vol in Volumes)
                vol.Clean();

            for (int i = Sources.Count - 1; i >= 0; i--)
            {
                foreach (Volume vol in Volumes)
                {
                    if (!vol.Bounds(Sources[i])) continue;

                    Assign(Sources[i], vol);
                    break;
                }
            }
        }

        public static void SetFocus(Volume volume)
        {
            if (!Volumes.Contains(volume)) return;

            Volumes.Remove(volume);
            Volumes.Insert(0, volume);
        }

        public static void Add(Volume volume)
        {
            if (!Volumes.Contains(volume))
                Volumes.Add(volume);
        }

        public static void Remove(Volume volume)
        {
            if (!Volumes.Contains(volume)) return;
            
            volume.Clear();
            Volumes.Remove(volume);
        }

        public static void Add(Source source)
        {
            if (Sources.Contains(source)) return;
            
            Sources.Add(source);
            source.Disassociate();
        }

        public static void Remove(Source source)
        {
            if (Sources.Contains(source))
                Sources.Remove(source);
        }

        public static void Assign(Source source, Volume volume)
        {
            if (!Sources.Contains(source)) return;

            Remove(source);
            volume.Add(source);
            source.Associate(volume);
        }

        #endregion
        
        
        #region Debug
        
        public static void DrawDebug()
        {
            foreach (Volume volume in Volumes)
                volume.DrawDebug();
            
            foreach (Source source in Sources)
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