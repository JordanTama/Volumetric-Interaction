using System;

namespace VolumetricInteraction.Benchmarking
{
    public class Parameter<T> : IParameter
    {
        private readonly T initial;

        private readonly Func<T, T> increment;
        private readonly Func<T, bool> completed;

        private T current;
            
        public Parameter(T initial, Func<T, T> increment, Func<T, bool> completed)
        {
            this.initial = initial;

            this.increment = increment;
            this.completed = completed;

            current = initial;
        }


        public static implicit operator T(Parameter<T> p) => p.current;

            
        public void Increment() => current = increment.Invoke(current);

        public void Reset() => current = initial;

        public bool Finished() => completed(current);
    }
}