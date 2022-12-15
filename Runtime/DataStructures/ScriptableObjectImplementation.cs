using UnityEngine;

namespace UsefulTools.Runtime.DataStructures
{
    public abstract class ScriptableObjectImplementation<T> : ScriptableObject where T : class
    {
        public T Implementation => this as T;
    }
}