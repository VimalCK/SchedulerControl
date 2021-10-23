using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Scheduler.Types
{
    internal sealed class OrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue> where TKey : notnull
    {
        private InternalKeyedCollection<TKey, KeyValuePair<TKey, TValue>> collection;

        public ICollection<TKey> Keys => collection.Select(c => c.Key).ToList();
        public ICollection<TValue> Values => collection.Select(c => c.Value).ToList();
        public int Count => collection.Count;
        public bool IsReadOnly => false;
        public TValue this[TKey key]
        {
            get => collection[key].Value;
            set
            {
                var keyValuePair = new KeyValuePair<TKey, TValue>(key, value);
                if (collection.TryGetValue(key, out KeyValuePair<TKey, TValue> item))
                {
                    item = keyValuePair;
                }
                else
                {
                    collection.Add(keyValuePair);
                }
            }
        }

        public TValue this[int index]
        {
            get => collection[index].Value;
            set => collection[index] = new(collection[index].Key, value);
        }

        public OrderedDictionary()
        {
            collection = new InternalKeyedCollection<TKey, KeyValuePair<TKey, TValue>>(item => item.Key);
        }

        public void Add(TKey key, TValue value) => collection.Add(new(key, value));
        public void Add(KeyValuePair<TKey, TValue> item) => collection.Add(item);
        public void Insert(int index, TKey key, TValue value) => collection.Insert(index, new(key, value));
        public void Clear() => collection.Clear();
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);
        public void RemoveAt(int index) => collection.RemoveAt(index);

        public bool Contains(KeyValuePair<TKey, TValue> item) => collection.Contains(item);
        public bool ContainsKey(TKey key) => collection.Contains(key);
        public bool Remove(TKey key) => collection.Remove(key);
        public bool Remove(KeyValuePair<TKey, TValue> item) => collection.Remove(item);
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (collection.Contains(key))
            {
                value = collection[key].Value;
                return true;
            }

            value = default;
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => collection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => collection.GetEnumerator();
    }
}
