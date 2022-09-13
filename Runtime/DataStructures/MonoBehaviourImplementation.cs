using UnityEngine;

namespace Tartaria.Codebase.DataStructures
{
    public abstract class MonoBehaviourImplementation<T> : MonoBehaviour
    {
        public abstract T Implementation { get; }
    }
}