using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace UsefulTools.Runtime.DataStructures.InterfaceImplementations
{
    [Serializable]
    public class ScriptableObjectImplementationList<T> : IDisposable, IImplementationList<T> where T : class
    {
        [SerializeField] private List<ScriptableObjectImplementation<T>> _list;

        public ScriptableObjectImplementationList()
        {
            _list = ListPool<ScriptableObjectImplementation<T>>.Get();
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

        public DisposableList<T> ToImplementationDisposableList()
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