using System;
using System.Collections.Generic;
using UnityEngine;

namespace UsefulTools.Runtime.Pools
{
    public class ArrayPool<T>
    {
        private readonly Queue<PooledArray> _arrays;
        private readonly int _length;

        public ArrayPool(int length)
        {
            _arrays = new Queue<PooledArray>();
            _length = length;
        }

        public PooledArray GetArray()
        {
            if (_arrays.Count > 0)
            {
                return _arrays.Dequeue();
            }

            return new PooledArray(_length, this);
        }

        public void Return(PooledArray array)
        {
            if (array.Length != _length)
            {
                Debug.LogError($"[ArrayPool]<color=red>{nameof(ArrayPool<T>)}.{nameof(Return)}></color> "
                               + "Returning array with different length: "
                               + $"array_length={array.Length}, "
                               + $"pool_length={_length}");
            }

            _arrays.Enqueue(array);
        }

        public class PooledArray : IDisposable
        {
            private readonly ArrayPool<T> _pool;

            public int Length => RawData.Length;
            public T[] RawData { get; }

            public PooledArray(int length, ArrayPool<T> pool)
            {
                RawData = new T[length];
                _pool = pool;
            }

            public T this[int i]
            {
                get => RawData[i];
                set => RawData[i] = value;
            }

            public void Dispose() => _pool.Return(this);
        }
    }
}