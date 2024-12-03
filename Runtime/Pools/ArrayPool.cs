using System;
using System.Collections;
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

		public class PooledArray : IEnumerable<T>, IDisposable
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

			private IEnumerator<T> _enumerator;
			public IEnumerator<T> GetEnumerator() => _enumerator ??= new PooledArrayEnumerator(this);
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}

		public class PooledArrayEnumerator : IEnumerator<T>
		{
			private readonly PooledArray _array;
			private int _position;

			public PooledArrayEnumerator(PooledArray array)
			{
				_array = array;
				Reset();
			}

			public void Reset() => _position = -1;

			public bool MoveNext()
			{
				_position++;
				return _position < _array.Length;
			}

			public T Current => _array[_position];
			object IEnumerator.Current => Current;

			public void Dispose() { }
		}
	}
}