using UnityEngine;

namespace UsefulTools.Runtime.DataStructures
{
    public abstract class MonoBehaviourImplementation<T> : MonoBehaviour
    {
        public abstract T Implementation { get; }
    }
}