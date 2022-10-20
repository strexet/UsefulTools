using System;
using UnityEngine;

namespace UsefulTools.Runtime.DataStructures
{
    [Serializable]
    public class ObservableVariable<T>
    {
        [SerializeField] private T _value;

        public event Action<T, T> OnValueChanged;

        public T Value
        {
            get => _value;
            set
            {
                var previous = _value;
                _value = value;
                OnValueChanged?.Invoke(previous, _value);
            }
        }

        public ObservableVariable(T @default) => _value = @default;

        public ObservableVariable() : this(default) { }
    }
}