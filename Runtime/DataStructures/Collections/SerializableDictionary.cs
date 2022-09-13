using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UsefulTools.Runtime.DataStructures.Collections
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : DrawableDictionary, IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [NonSerialized] private Dictionary<TKey, TValue> _dict;
        [NonSerialized] private IEqualityComparer<TKey> _comparer;

        public IEqualityComparer<TKey> Comparer => _comparer;

        public SerializableDictionary() { }

        public SerializableDictionary(IEqualityComparer<TKey> comparer) =>
            _comparer = comparer;

#region IDictionary Interface
        public int Count => _dict != null ? _dict.Count : 0;

        public void Add(TKey key, TValue value)
        {
            if (_dict == null)
            {
                _dict = new Dictionary<TKey, TValue>(Comparer);
            }

            _dict.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            if (_dict == null)
            {
                return false;
            }

            return _dict.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                if (_dict == null)
                {
                    _dict = new Dictionary<TKey, TValue>(Comparer);
                }

                return _dict.Keys;
            }
        }

        public bool Remove(TKey key)
        {
            if (_dict == null)
            {
                return false;
            }

            return _dict.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_dict == null)
            {
                value = default;
                return false;
            }

            return _dict.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get
            {
                if (_dict == null)
                {
                    _dict = new Dictionary<TKey, TValue>(Comparer);
                }

                return _dict.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (_dict == null)
                {
                    throw new KeyNotFoundException();
                }

                return _dict[key];
            }
            set
            {
                if (_dict == null)
                {
                    _dict = new Dictionary<TKey, TValue>(Comparer);
                }

                _dict[key] = value;
            }
        }

        public void Clear()
        {
            if (_dict != null)
            {
                _dict.Clear();
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            if (_dict == null)
            {
                _dict = new Dictionary<TKey, TValue>(Comparer);
            }

            (_dict as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            if (_dict == null)
            {
                return false;
            }

            return (_dict as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (_dict == null)
            {
                return;
            }

            (_dict as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_dict == null)
            {
                return false;
            }

            return (_dict as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            if (_dict == null)
            {
                return default;
            }

            return _dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_dict == null)
            {
                return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            }

            return _dict.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            if (_dict == null)
            {
                return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            }

            return _dict.GetEnumerator();
        }
#endregion

#region ISerializationCallbackReceiver
        [SerializeField] private TKey[] _Keys;
        [SerializeField] private TValue[] _Values;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_Keys != null && _Values != null)
            {
                if (_dict == null)
                {
                    _dict = new Dictionary<TKey, TValue>(_Keys.Length, Comparer);
                }
                else
                {
                    _dict.Clear();
                }

                for (int i = 0; i < _Keys.Length; i++)
                {
                    if (i < _Values.Length)
                    {
                        _dict[_Keys[i]] = _Values[i];
                    }
                    else
                    {
                        _dict[_Keys[i]] = default;
                    }
                }
            }

            _Keys = null;
            _Values = null;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_dict == null || _dict.Count == 0)
            {
                _Keys = null;
                _Values = null;
            }
            else
            {
                int count = _dict.Count;
                _Keys = new TKey[count];
                _Values = new TValue[count];

                int i = 0;
                var enumerator = _dict.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    _Keys[i] = enumerator.Current.Key;
                    _Values[i] = enumerator.Current.Value;
                    i++;
                }
            }
        }
#endregion
    }
}