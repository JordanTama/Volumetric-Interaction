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
        
        private State _state;
        private Type _type = Type.Default;
        private SettingsProfile _profile;


        private enum Type
        {
            Default,
            Decay,
            BruteForce
        }

        private void StartBenchmark()
        {
            switch (_type)
            {
                case Type.Default:
                    _profile.useDecay = false;
                    _profile.useBruteForce = false;
                    break;
                
                case Type.Decay:
                    _profile.useDecay = true;
                    _profile.useBruteForce = false;
                    break;
                
                case Type.BruteForce:
                    _profile.useDecay = false;
                    _profile.useBruteForce = true;
                    break;
            }

            BeginRound();
        }
        
        public void Benchmark()
        {
            _profile = ScriptableObject.CreateInstance<SettingsProfile>();
            _profile.ApplyValues(Settings.Profile);
            
            Parameter<Vector3Int> resolutionParam = new Parameter<Vector3Int>(new Vector3Int(0, 0, 0),
                v => v + new Vector3Int(32, 32, 32), v => v.Equals(new Vector3Int(256, 256, 256)));

            Parameter<int> sourceCountParam = new Parameter<int>(0, v => v + 25, v => v >= 100);

            Parameter<float> timeStepParam = new Parameter<float>(0f, v => v + 0.025f, v => v >= 0.1f);

            _state = new State(resolutionParam, sourceCountParam, timeStepParam);
            _type = Type.Default;
            
            StartBenchmark();
        }
        
        private void BeginRound()
        {
            _profile.resolution = _state.Resolution;
            _profile.timeStep = _state.TimeStep;
            
            Settings.ApplyValues(_profile);
            
            Logger.Begin(_state);
            
            tunnel.Initialize(OnRoundEnd, _state.SourceCount, roundDuration, roundSeed);
        }

        private void OnRoundEnd()
        {
            Logger.End();

            _state.Increment();

            if (_state.Completed)
            {
                _type++;
                if ((int)_type < Enum.GetValues(typeof(Type)).Length)
                    StartBenchmark();
                return;
            }
            
            BeginRound();
        }
    }
}