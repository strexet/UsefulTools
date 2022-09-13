using System;
using System.Collections.Generic;
using UnityEngine;

namespace UsefulTools.Runtime
{
    public class Pool
    {
        private static Pool _instance;
        private static Pool Instance => _instance ??= new Pool();

        private readonly Dictionary<Type, ObjectPool> _pools;

        private Pool() => _pools = new Dictionary<Type, ObjectPool>();

        public static T Get<T>() where T : new()
        {
            if (Instance.GetFromPool(typeof(T), out object fromPool))
            {
                return (T)fromPool;
            }

            return new T();
        }

        public static void Release(object poolable)
        {
            if (poolable == null)
            {
                return;
            }

            Instance.ReturnToPool(poolable.GetType(), poolable);
        }

        private bool GetFromPool(Type type, out object fromPool)
        {
            var pool = GetPool(type);

            if (pool.Count > 0)
            {
                fromPool = pool.Get();
                return true;
            }

            fromPool = default;
            return false;
        }

        private void ReturnToPool(Type type, object poolable)
        {
            var pool = GetPool(type);
            pool.Return(poolable);
        }

        private ObjectPool GetPool(Type type)
        {
            if (_pools.TryGetValue(type, out var pool))
            {
                return pool;
            }

            pool = new ObjectPool();
            _pools.Add(type, pool);

            return pool;
        }

        private class ObjectPool
        {
            private readonly Stack<object> pooled;
            private readonly HashSet<object> pooledSet;

            public int Count { get; private set; }

            public ObjectPool()
            {
                pooled = new Stack<object>();
                pooledSet = new HashSet<object>();
                Count = 0;
            }

            public object Get()
            {
                object fromPool = pooled.Pop();
                pooledSet.Remove(fromPool);

                Count--;

                return fromPool;
            }

            public void Return(object poolable)
            {
                if (pooledSet.Contains(poolable))
                {
                    Debug.LogError(
                        $"{nameof(ObjectPool)}.{nameof(ReturnToPool)}> Poolable object is already returned to pool: {poolable.GetType().Name}");

                    return;
                }

                if (poolable is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                pooledSet.Add(poolable);
                pooled.Push(poolable);

                Count++;
            }
        }
    }
}