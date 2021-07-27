using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace ChangeInput
{
    public class CircularDictionary<T1, T2> : IDictionary<T1, T2>
    {
        private Dictionary<T1, T2> _uniqueDictionary = new Dictionary<T1, T2>();
        private HashSet<T2> _values = new HashSet<T2>();

        public T2 this[T1 key]
        {
            get => _uniqueDictionary[key];
            set
            {
                if (ContainsValue(value))
                {
                    throw new ArgumentException($"Instance of UniqueDictionary already contains a value of \"{value}\"!");
                }
                _uniqueDictionary[key] = value;
            }
        }
        public ICollection<T1> Keys => _uniqueDictionary.Keys;
        public ICollection<T2> Values => _uniqueDictionary.Values;
        public int Count => _uniqueDictionary.Count;
        public bool IsReadOnly => false;

        public void Add(T1 key, T2 value)
        {
            if (ContainsValue(value))
            {
                throw new ArgumentException($"Instance of UniqueDictionary already contains a value of \"{value}\"!");
            }
            _uniqueDictionary.Add(key, value);
            _values.Add(value);
        }
        public void Add(KeyValuePair<T1, T2> item)
        {
            if (ContainsValue(item.Value))
            {
                throw new ArgumentException($"Instance of UniqueDictionary already contains a value of \"{item.Value}\"!");
            }
            _uniqueDictionary.Add(item.Key, item.Value);
            _values.Add(item.Value);
        }
        public bool Remove(T1 key)
        {
            if (ContainsKey(key))
            {
                _values.Remove(_uniqueDictionary[key]);
            }
            return _uniqueDictionary.Remove(key);
        }
        public bool Remove(KeyValuePair<T1, T2> item)
        {
            if (ContainsKey(item.Key))
            {
                _values.Remove(_uniqueDictionary[item.Key]);
            }
            return _uniqueDictionary.Remove(item.Key);
        }
        public bool Contains(KeyValuePair<T1, T2> item)
        {
            return _uniqueDictionary.Contains(item);
        }
        public bool ContainsKey(T1 key)
        {
            return _uniqueDictionary.ContainsKey(key);
        }
        public bool ContainsValue(T2 value)
        {
            return _values.Contains(value);
        }
        public T1 GetKey(T2 value)
        {
            return _uniqueDictionary.FirstOrDefault(x => x.Value.Equals(value)).Key;
        }
        public bool TryGetValue(T1 key, out T2 value)
        {
            return _uniqueDictionary.TryGetValue(key, out value);
        }
        public bool TryGetKey(T2 value, out T1 key)
        {
            key = GetKey(value);
            return !key.Equals(default);
        }
        public void Clear()
        {
            _uniqueDictionary.Clear();
        }
        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
        {
            array = _uniqueDictionary.Skip(arrayIndex + 1).ToArray();
        }
        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return _uniqueDictionary.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _uniqueDictionary.GetEnumerator();
        }
    }
}
