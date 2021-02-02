using System;

namespace VolumetricInteraction.Benchmarking
{
    public class Parameter<T>
    {
        private readonly T initial;
        private readonly T target;

        private readonly Func<T, T> increment;

        private T current;
            
        public Parameter(T initial, T target, Func<T, T> increment)
        {
            this.initial = initial;
            this.target = target;

            this.increment = increment;

            current = initial;
        }


        public static implicit operator T(Parameter<T> p) => p.current;

            
        public void Increment() => current = increment.Invoke(current);

        public void Reset() => current = initial;

        public bool Finished() => current.Equals(target);
    }
}