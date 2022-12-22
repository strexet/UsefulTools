using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace UsefulTools.Runtime.DataStructures
{
    [Serializable]
    public class DisposableList<T> : IDisposable
    {
        [SerializeField] private List<T> _list;

        public IReadOnlyList<T> ReadOnlyList => _list;
        public List<T> List => _list;

        public DisposableList()
        {
            _list = ListPool<T>.Get();
        }

        public void Dispose()
        {
            ListPool<T>.Release(_list);
        }
    }
}