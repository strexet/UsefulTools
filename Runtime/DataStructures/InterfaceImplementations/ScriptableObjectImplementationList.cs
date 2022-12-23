using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace UsefulTools.Runtime.DataStructures.InterfaceImplementations
{
    [Serializable]
    public class ScriptableObjectImplementationList<T> : IDisposable, IConvertableImplementationList<T> where T : class
    {
        [SerializeField] private List<ScriptableObjectImplementation<T>> _list;

        public int Count => _list.Count;

        public ScriptableObjectImplementationList()
        {
            _list = ListPool<ScriptableObjectImplementation<T>>.Get();
        }

        public void Add(T item)
        {
            if (item is ScriptableObjectImplementation<T> implementation)
            {
                _list.Add(implementation);
            }
            else
            {
                Debug.LogError(
                    $"Trying to add not implementation item ({item.GetType()}) in {nameof(ScriptableObjectImplementation<T>)}<{typeof(T)}>");
            }
        }

        public bool Contains(T item)
        {
            foreach (var implementation in _list)
            {
                if (item.Equals(implementation.Implementation))
                {
                    return true;
                }
            }

            return false;
        }

        public List<T> ToImplementationList()
        {
            var list = ListPool<T>.Get();

            foreach (var item in _list)
            {
                if (item != null)
                {
                    list.Add(item.Implementation);
                }
            }

            return list;
        }

        public List<TParent> ToParentImplementationList<TParent>()
        {
            var list = ListPool<TParent>.Get();

            foreach (var item in _list)
            {
                if (item != null && item.Implementation is TParent asParent)
                {
                    list.Add(asParent);
                }
                else
                {
                    Debug.LogError($"Not parent item ({item.GetType()}) of {nameof(ScriptableObjectImplementation<T>)}<{typeof(T)}>");
                    return list;
                }
            }

            return list;
        }

        public List<TChild> ToChildImplementationList<TChild>() where TChild : T
        {
            var list = ListPool<TChild>.Get();

            foreach (var item in _list)
            {
                if (item != null && item.Implementation is TChild asChild)
                {
                    list.Add(asChild);
                }
                else
                {
                    Debug.LogError($"Not child item ({item.GetType()}) of {nameof(ScriptableObjectImplementation<T>)}<{typeof(T)}>");
                    return list;
                }
            }

            return list;
        }

        public void Dispose()
        {
            ListPool<ScriptableObjectImplementation<T>>.Release(_list);
        }
    }
}