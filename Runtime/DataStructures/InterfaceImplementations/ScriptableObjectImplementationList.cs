using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace UsefulTools.Runtime.DataStructures.InterfaceImplementations
{
    [Serializable]
    public class ScriptableObjectImplementationList<T> : IDisposable where T : class
    {
        [SerializeField] private List<ScriptableObjectImplementation<T>> _list;

        public ScriptableObjectImplementationList()
        {
            _list = ListPool<ScriptableObjectImplementation<T>>.Get();
        }

        [Obsolete]
        public List<T> ToList()
        {
            var list = ListPool<T>.Get();

            foreach (var item in _list)
            {
                list.Add(item.Implementation);
            }

            return list;
        }

        public DisposableList<T> ToDisposableList()
        {
            var disposableList = new DisposableList<T>();

            foreach (var item in _list)
            {
                disposableList.List.Add(item.Implementation);
            }

            return disposableList;
        }

        public void Dispose()
        {
            ListPool<ScriptableObjectImplementation<T>>.Release(_list);
        }
    }
}