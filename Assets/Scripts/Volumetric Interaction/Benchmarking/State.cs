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

        
        public bool Completed { get; private set; }
            
        private List<IParameter> Parameters => new List<IParameter> { Resolution, SourceCount, TimeStep };

        
        public State(Parameter<Vector3Int> resolution, Parameter<int> sourceCount, Parameter<float> timeStep)
        {
            Resolution = resolution;
            SourceCount = sourceCount;
            TimeStep = timeStep;

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

        public override string ToString()
        {
            return ((Vector3Int) Resolution).ToString() + ", " + (int) SourceCount + ", " + (float) TimeStep;
        }
    }
}