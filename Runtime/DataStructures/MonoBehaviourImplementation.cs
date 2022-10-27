using UnityEngine;

namespace UsefulTools.Runtime.DataStructures
{
    public abstract class MonoBehaviourImplementation<T> : MonoBehaviour where T : class
    {
        public T Implementation => this as T;
    }
}