using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VolumetricInteraction
{
    // High-priority
    // TODO: Implement a way of stepping through the texture generation gradually to visualise the process.
    // TODO: Create test scripts/timelines
    // TODO: BENCHMARKING
    
    // Low-priority
    // BUG: When not generating in editor, the VI texture shows as grey.
    // BUG: Source doesn't draw (but IS managed) when on the POSITIVE bounds of the volume.
    // TODO: Maybe only recalculate buffer if a source has been added/removed?
    // TODO: Find dynamic approach to efficiently managing thread group sizes.
    public static class Core
    {
        private static readonly List<Volume> Volumes = new List<Volume>();
        private static readonly List<Source> Sources = new List<Source>();

        private static RenderTexture _texture;
        private static RenderTexture _previous;
        private static ComputeBuffer _buffer;
        
        private static readonly int InteractionTexture = Shader.PropertyToID("interaction_texture");
        private static readonly int VolumeLocalToWorld = Shader.PropertyToID("volume_local_to_world");
        private static readonly int VolumeWorldToLocal = Shader.PropertyToID("volume_world_to_local");

        
        public static Volume FocusVolume => Volumes.Count > 0 ? Volumes[0] : null;


        private enum Kernel
        {
            BruteForce,
            Sentinel,
            Seeding,
            JumpFlooding,
            Conversion,
            Decay,
            Blending
        }
        

        #region Event Functions

        public static void Initialize()
        {
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

            if (!Settings.GenerateInEditor && !Application.isPlaying)
                return;
                
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
            CreateTexture(out _texture);
            CreateTexture(out _previous);
        }

        private static void CreateTexture(out RenderTexture texture)
        {
            texture =
                new RenderTexture(Settings.Resolution.x, Settings.Resolution.y, 0, RenderTextureFormat.ARGB32)
                {
                    dimension = TextureDimension.Tex3D,
                    volumeDepth = Settings.Resolution.z,
                    wrapMode = TextureWrapMode.Clamp,
                    enableRandomWrite = true,
                    filterMode = Settings.FilterMode
                };

            texture.Create();
        }

        private static void UpdateTexture(float delta)
        {
            if (Settings.UseDecay)
                Graphics.CopyTexture(_texture, _previous);
            
            Vector3Int threadGroups = new Vector3Int(_texture.width / 8, _texture.height / 8, _texture.volumeDepth / 8);

            // STEP 1. Sentinel pass.
            Settings.Shader.SetTexture((int) Kernel.Sentinel, "current", _texture);
            Settings.Shader.Dispatch((int) Kernel.Sentinel, threadGroups.x, threadGroups.y, threadGroups.z);

            // STEP 2. Vector field generation.
            if (FocusVolume && FocusVolume.Count > 0)
            {
                // STEP 2-1. Construct compute buffer.
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
            
                // STEP 2-2. Update global compute shader variables.
                Settings.Shader.SetMatrix("volume_local_to_world", FocusVolume.transform.localToWorldMatrix);
                Settings.Shader.SetMatrix("volume_world_to_local", FocusVolume.transform.worldToLocalMatrix);
                Settings.Shader.SetInts("resolution", _texture.width, _texture.height, _texture.volumeDepth);
                Settings.Shader.SetFloat("radius_multiplier", multiplier);

                // STEP 2-3. Run main texture generation method.
                if (Settings.UseBruteForce)
                    BruteUpdateTexture(threadGroups);
                else
                    FloodUpdateTexture(threadGroups);
                
                Shader.SetGlobalMatrix(VolumeLocalToWorld, FocusVolume.transform.localToWorldMatrix);
                Shader.SetGlobalMatrix(VolumeWorldToLocal, FocusVolume.transform.worldToLocalMatrix);
            
                // STEP 2-4. Release the buffer.
                _buffer.Release();
            }

            // STEP 3. Decay passes.
            if (Settings.UseDecay)
            {
                // STEP 3. Decay pass.
                Settings.Shader.SetFloat("delta", delta);
                Settings.Shader.SetFloat("decay_speed", Settings.DecaySpeed);
                Settings.Shader.SetTexture((int) Kernel.Decay, "previous", _previous);

                Settings.Shader.Dispatch((int) Kernel.Decay, threadGroups.x, threadGroups.y, threadGroups.z);

                // STEP 4. Blend pass.
                Settings.Shader.SetTexture((int) Kernel.Blending, "current", _texture);
                Settings.Shader.SetTexture((int) Kernel.Blending, "previous", _previous);

                Settings.Shader.Dispatch((int) Kernel.Blending, threadGroups.x, threadGroups.y, threadGroups.z);
            }

            Shader.SetGlobalTexture(InteractionTexture, _texture);
        }
        
        private static void BruteUpdateTexture(Vector3Int threadGroups)
        {
            Settings.Shader.SetTexture((int) Kernel.BruteForce, "current", _texture);
            Settings.Shader.SetBuffer((int) Kernel.BruteForce, "buffer", _buffer);
            
            // Dispatch compute shader
            Settings.Shader.Dispatch((int) Kernel.BruteForce, threadGroups.x, threadGroups.y, threadGroups.z);
        }

        private static void FloodUpdateTexture(Vector3Int threadGroups)
        {
            // STEP 2. Seeding pass.
            Settings.Shader.SetTexture((int) Kernel.Seeding, "current", _texture);
            Settings.Shader.SetBuffer((int) Kernel.Seeding, "buffer", _buffer);

            Settings.Shader.Dispatch((int) Kernel.Seeding, _buffer.count, 1, 1);
            
            // STEP 3. Jump Flooding Algorithm pass.
            Settings.Shader.SetTexture((int) Kernel.JumpFlooding, "current", _texture);

            Vector3Int step = new Vector3Int(_texture.width, _texture.height, _texture.volumeDepth);
            while (step.x > 1 || step.y > 1 || step.z >  1)
            {
                step.x = Mathf.Max(1, step.x / 2);
                step.y = Mathf.Max(1, step.y / 2);
                step.z = Mathf.Max(1, step.z / 2);
                
                Settings.Shader.SetInts("step_size", step.x, step.y, step.z);
                
                Settings.Shader.Dispatch((int) Kernel.JumpFlooding, threadGroups.x, threadGroups.y, threadGroups.z);
            }
            
            // STEP 4. Conversion pass.
            Settings.Shader.SetTexture((int) Kernel.Conversion, "current", _texture);

            Settings.Shader.Dispatch((int) Kernel.Conversion, threadGroups.x, threadGroups.y, threadGroups.z);
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

        private static void Assign(Source source, Volume volume)
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