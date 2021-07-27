using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace ChangeInput.Core
{
    public class CircularDictionary<T1, T2> : IDictionary<T1, T2>
    {
        private Dictionary<T1, T2> _circularDictionary = new Dictionary<T1, T2>();
        private HashSet<T2> _values = new HashSet<T2>();

        public T2 this[T1 key]
        {
            get => _circularDictionary[key];
            set
            {
                if (ContainsValue(value))
                {
                    throw new ArgumentException($"Instance of UniqueDictionary already contains a value of \"{value}\"!");
                }
                else if (!ContainsKey(key))
                {
                    Add(key, value);
                }
                else
                {
                    _circularDictionary[key] = value;
                }
            }
        }
        public ICollection<T1> Keys => _circularDictionary.Keys;
        public ICollection<T2> Values => _circularDictionary.Values;
        public int Count => _circularDictionary.Count;
        public bool IsReadOnly => false;

        public void Add(T1 key, T2 value)
        {
            if (ContainsValue(value))
            {
                throw new ArgumentException($"Instance of UniqueDictionary already contains a value of \"{value}\"!");
            }
            _circularDictionary.Add(key, value);
            _values.Add(value);
        }
        public void Add(KeyValuePair<T1, T2> item)
        {
            if (ContainsValue(item.Value))
            {
                throw new ArgumentException($"Instance of UniqueDictionary already contains a value of \"{item.Value}\"!");
            }
            _circularDictionary.Add(item.Key, item.Value);
            _values.Add(item.Value);
        }
        public bool Remove(T1 key)
        {
            if (ContainsKey(key))
            {
                _values.Remove(_circularDictionary[key]);
            }
            return _circularDictionary.Remove(key);
        }
        public bool Remove(KeyValuePair<T1, T2> item)
        {
            if (ContainsKey(item.Key))
            {
                _values.Remove(_circularDictionary[item.Key]);
            }
            return _circularDictionary.Remove(item.Key);
        }
        public bool Contains(KeyValuePair<T1, T2> item)
        {
            return _circularDictionary.Contains(item);
        }
        public bool ContainsKey(T1 key)
        {
            return _circularDictionary.ContainsKey(key);
        }
        public bool ContainsValue(T2 value)
        {
            return _values.Contains(value);
        }
        public T1 GetKey(T2 value)
        {
            return _circularDictionary.FirstOrDefault(x => x.Value.Equals(value)).Key;
        }
        public bool TryGetValue(T1 key, out T2 value)
        {
            return _circularDictionary.TryGetValue(key, out value);
        }
        public bool TryGetKey(T2 value, out T1 key)
        {
            key = GetKey(value);
            return !key.Equals(default);
        }
        public void Clear()
        {
            _circularDictionary.Clear();
        }
        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
        {
            array = _circularDictionary.Skip(arrayIndex + 1).ToArray();
        }
        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return _circularDictionary.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _circularDictionary.GetEnumerator();
        }
    }
}
