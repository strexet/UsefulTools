using UnityEngine;

namespace UsefulTools.Runtime.DataStructures.InterfaceImplementations
{
    public abstract class ScriptableObjectImplementation<T> : ScriptableObject, IInterfaceImplementation<T> where T : class
    {
        public T Implementation => this as T;
    }
}