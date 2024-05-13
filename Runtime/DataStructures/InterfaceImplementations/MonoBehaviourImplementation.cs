using UnityEngine;

namespace UsefulTools.Runtime.DataStructures.InterfaceImplementations
{
    public abstract class MonoBehaviourImplementation<T> : MonoBehaviour, IInterfaceImplementation<T> where T : class
    {
        public T Implementation => this as T;
    }
}