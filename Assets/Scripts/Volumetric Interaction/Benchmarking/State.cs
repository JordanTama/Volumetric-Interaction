using System;
using System.Collections.Generic;
using UnityEngine;

namespace VolumetricInteraction.Benchmarking
{
    [Serializable]
    public struct State
    {
        public Parameter<Vector3Int> Resolution;
        public Parameter<int> SourceCount;
        public Parameter<float> TimeStep;
        public Parameter<bool> UseDecay;
        public Parameter<bool> UseBruteForce;

        
        public bool Completed { get; private set; }
        private List<IParameter> Parameters => new List<IParameter> { UseDecay, UseBruteForce, Resolution, SourceCount, TimeStep };

        
        public State(Parameter<Vector3Int> resolution, Parameter<int> sourceCount, Parameter<float> timeStep, Parameter<bool> useDecay, Parameter<bool> useBruteForce)
        {
            Resolution = resolution;
            SourceCount = sourceCount;
            TimeStep = timeStep;
            UseDecay = useDecay;
            UseBruteForce = useBruteForce;

            Completed = false;
        }


        public void Increment()
        {
            if (Completed)
                return;
            
            List<IParameter> parameters = Parameters;
         
            for (int i = 0; i < parameters.Count; i++)
            {
                if (!parameters[i].Finished())
                {
                    parameters[i].Increment();
                    break;
                }

                if (i == parameters.Count - 1)
                {
                    Completed = true;
                    break;
                }
                
                parameters[i].Reset();
            }
        }

        public void Reset()
        {
            Completed = false;
            
            foreach (IParameter p in Parameters)
                p.Reset();
        }
    }
}