using System;
using UnityEngine;

namespace VolumetricInteraction.Benchmarking
{
    // TODO: Implement settings iteration.
    // TODO: Implement logging functionality.
    // TODO: Implement log output.
    public class Controller : MonoBehaviour
    {
        [SerializeField] private Tunnel tunnel;
        [SerializeField] private float roundDuration;
        [SerializeField] private int roundSeed;
        
        private Settings _settings;
        private SettingsProfile _profile;

        [ContextMenu("Benchmark")]
        public void Benchmark()
        {
            _profile = ScriptableObject.CreateInstance<SettingsProfile>();
            _profile.ApplyValues(VolumetricInteraction.Settings.Profile);

            Parameter<Vector3Int> resolutionParam = new Parameter<Vector3Int>(new Vector3Int(32, 32, 32),
                new Vector3Int(256, 256, 256), v => v + new Vector3Int(32, 32, 32));

            Parameter<int> sourceCountParam = new Parameter<int>(25, 100, v => v + 25);

            _settings = new Settings(resolutionParam, sourceCountParam);

            BeginRound();
        }
        
        private void BeginRound()
        {
            _profile.resolution = _settings.Resolution;
            VolumetricInteraction.Settings.ApplyValues(_profile);
            
            tunnel.Initialize(OnRoundEnd, _settings.SourceCount, roundDuration, roundSeed);
        }

        private void OnRoundEnd()
        {
            if (_settings.Resolution.Finished() && _settings.SourceCount.Finished())
                return;
            
            if (_settings.SourceCount.Finished())
            {
                _settings.SourceCount.Reset();
                _settings.Resolution.Increment();
            }
            else
            {
                _settings.SourceCount.Increment();
            }
            
            BeginRound();
        }
        
        
    }
}