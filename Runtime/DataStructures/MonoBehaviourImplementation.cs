using UnityEngine;

namespace UsefulTools.Runtime.DataStructures
{
    public abstract class MonoBehaviourImplementation<T> : MonoBehaviour
    {
        public T Implementation => this;

        public static implicit operator T(MonoBehaviourImplementation<T> implementation) => implementation.Implementation;
    }
}