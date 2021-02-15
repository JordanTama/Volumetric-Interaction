using System;
using UnityEngine;

namespace VolumetricInteraction.Benchmarking
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private Tunnel tunnel;
        [SerializeField] private float roundDuration;
        [SerializeField] private int roundSeed;
        [SerializeField] private SettingsProfile[] profiles;
        
        private State _state;
        private SettingsProfile _profile;

        private static SettingsProfile[] _profiles;
        private static int _profileIndex;

        public static TestType Test = TestType.Performance;


        public enum TestType
        {
            Performance,
            Decay,
            BruteForceSource,
            BruteForceRes,
            TimeStep,
            QuarterRes
        }


        private void Start()
        {
            _profiles = profiles;
        }

        
        public void Benchmark()
        {
            _profile = ScriptableObject.CreateInstance<SettingsProfile>();
            _profile.ApplyValues(Settings.Profile);

            _state = CreateState(Test);
            
            BeginRound();
        }

        private static State CreateState(TestType test)
        {
            State state;
            
            switch (test)
            {
                case TestType.Performance:
                    state = Performance();
                    break;
                
                case TestType.Decay:
                    state = Decay();
                    break;
                
                case TestType.BruteForceSource:
                    state = BruteForceSource();
                    break;
                
                case TestType.BruteForceRes:
                    state = BruteForceRes();
                    break;
                
                case TestType.TimeStep:
                    state = TimeStep();
                    break;
                
                case TestType.QuarterRes:
                    state = QuarterRes();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return state;

            State Performance()
            {
                Parameter<Vector3Int> resolutionParam = new Parameter<Vector3Int>(Vector3Int.zero,
                    i => i + Vector3Int.one * 16, i => i == Vector3Int.one * 128);

                Parameter<int> sourceCountParam = new Parameter<int>(20, i => i, i => true);

                Parameter<float> timeStepParam = new Parameter<float>(0f, f => f, i => true);

                Parameter<bool> useDecayParam = new Parameter<bool>(true, b => b, b => true);
                
                Parameter<bool> useBruteForceParam = new Parameter<bool>(false, b => b, b => true);

                return new State(resolutionParam, sourceCountParam, timeStepParam, useDecayParam, useBruteForceParam);
            }
            
            State Decay()
            {
                Parameter<Vector3Int> resolutionParam = new Parameter<Vector3Int>(Vector3Int.zero,
                    i => i + Vector3Int.one * 16, i => i == Vector3Int.one * 128);

                Parameter<int> sourceCountParam = new Parameter<int>(20, i => i, i => true);

                Parameter<float> timeStepParam = new Parameter<float>(0f, f => f, i => true);

                Parameter<bool> useDecayParam = new Parameter<bool>(true, b => !b, b => !b);
                
                Parameter<bool> useBruteForceParam = new Parameter<bool>(false, b => b, b => true);

                return new State(resolutionParam, sourceCountParam, timeStepParam, useDecayParam, useBruteForceParam);
            }

            State BruteForceSource()
            {
                Parameter<Vector3Int> resolutionParam = new Parameter<Vector3Int>(Vector3Int.one * 64,
                    i => i, i => true);

                Parameter<int> sourceCountParam = new Parameter<int>(0, i => i + 50, i => i >= 400);

                Parameter<float> timeStepParam = new Parameter<float>(0f, f => f, i => true);

                Parameter<bool> useDecayParam = new Parameter<bool>(true, b => b, b => true);
                
                Parameter<bool> useBruteForceParam = new Parameter<bool>(true, b => !b, b => !b);

                return new State(resolutionParam, sourceCountParam, timeStepParam, useDecayParam, useBruteForceParam);
            }

            State BruteForceRes()
            {
                Parameter<Vector3Int> resolutionParam = new Parameter<Vector3Int>(Vector3Int.zero,
                    i => i + Vector3Int.one * 16, i => i == Vector3Int.one * 128);

                Parameter<int> sourceCountParam = new Parameter<int>(50, i => i, i => true);

                Parameter<float> timeStepParam = new Parameter<float>(0f, f => f, i => true);

                Parameter<bool> useDecayParam = new Parameter<bool>(true, b => b, b => true);
                
                Parameter<bool> useBruteForceParam = new Parameter<bool>(true, b => !b, b => !b);

                return new State(resolutionParam, sourceCountParam, timeStepParam, useDecayParam, useBruteForceParam);
            }

            State TimeStep()
            {
                Parameter<Vector3Int> resolutionParam = new Parameter<Vector3Int>(Vector3Int.one * 64,
                    i => i, i => true);

                Parameter<int> sourceCountParam = new Parameter<int>(20, i => i, i => true);

                Parameter<float> timeStepParam = new Parameter<float>(0f, f => f + 0.01f, i => i >= 0.1f);

                Parameter<bool> useDecayParam = new Parameter<bool>(true, b => b, b => true);
                
                Parameter<bool> useBruteForceParam = new Parameter<bool>(false, b => b, b => true);

                return new State(resolutionParam, sourceCountParam, timeStepParam, useDecayParam, useBruteForceParam);
            }
            
            State QuarterRes()
            {
                Parameter<Vector3Int> resolutionParam = new Parameter<Vector3Int>(Vector3Int.zero,
                    Increment, i => i == new Vector3Int(192, 48, 192));

                Parameter<int> sourceCountParam = new Parameter<int>(20, i => i, i => true);

                Parameter<float> timeStepParam = new Parameter<float>(0f, f => f, i => true);

                Parameter<bool> useDecayParam = new Parameter<bool>(true, b => b, b => true);
                
                Parameter<bool> useBruteForceParam = new Parameter<bool>(false, b => b, b => true);

                return new State(resolutionParam, sourceCountParam, timeStepParam, useDecayParam, useBruteForceParam);
                
                Vector3Int Increment(Vector3Int v)
                {
                    if (v.y == v.x / 4)
                        v = new Vector3Int(v.x, v.x, v.x) + Vector3Int.one * 32;
                    else
                        v.y /= 4;

                    return v;
                }
            }
        }
        
        private void BeginRound()
        {
            _profile.resolution = _state.Resolution;
            _profile.timeStep = _state.TimeStep;
            _profile.useDecay = _state.UseDecay;
            _profile.useBruteForce = _state.UseBruteForce;
            
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
                if ((int) ++Test < Enum.GetValues(typeof(TestType)).Length)
                    Benchmark();
                
                return;
            }
            
            BeginRound();
        }
    }
}