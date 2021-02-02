using System;
using UnityEngine;

namespace VolumetricInteraction.Benchmarking
{
    // TODO: Implement logging functionality.
    // TODO: Implement log output.
    public class Controller : MonoBehaviour
    {
        [SerializeField] private Tunnel tunnel;
        [SerializeField] private float roundDuration;
        [SerializeField] private int roundSeed;
        
        private State state;
        private SettingsProfile _profile;

        [ContextMenu("Benchmark")]
        public void Benchmark()
        {
            _profile = ScriptableObject.CreateInstance<SettingsProfile>();
            _profile.ApplyValues(Settings.Profile);

            Parameter<Vector3Int> resolutionParam = new Parameter<Vector3Int>(new Vector3Int(32, 32, 32),
                v => v + new Vector3Int(32, 32, 32), v => v.Equals(new Vector3Int(128, 128, 128)));

            Parameter<int> sourceCountParam = new Parameter<int>(0, v => v + 25, v => v >= 100);

            Parameter<float> timeStepParam = new Parameter<float>(0f, v => v + 0.05f, v => v >= 0.1f);

            state = new State(resolutionParam, sourceCountParam, timeStepParam);

            BeginRound();
        }
        
        private void BeginRound()
        {
            _profile.resolution = state.Resolution;
            _profile.timeStep = state.TimeStep;
            
            Settings.ApplyValues(_profile);
            
            Logger.Begin(state);
            
            tunnel.Initialize(OnRoundEnd, state.SourceCount, roundDuration, roundSeed);
        }

        private void OnRoundEnd()
        {
            Logger.End();
            
            if (state.Completed)
                return;

            state.Increment();
            
            BeginRound();
        }
    }
}