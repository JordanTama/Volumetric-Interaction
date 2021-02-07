using System;

namespace VolumetricInteraction.Benchmarking
{
    public class Parameter<T> : IParameter
    {
        private readonly T _initial;

        private readonly Func<T, T> _increment;
        private readonly Func<T, bool> _completed;

        private T _current;
            
        public Parameter(T initial, Func<T, T> increment, Func<T, bool> completed)
        {
            _initial = initial;

            _increment = increment;
            _completed = completed;

            _current = initial;
        }


        public static implicit operator T(Parameter<T> p) => p._current;

            
        public void Increment() => _current = _increment.Invoke(_current);

        public void Reset() => _current = _initial;

        public bool Finished() => _completed(_current);
    }
}