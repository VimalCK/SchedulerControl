using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Scheduler.Types
{
    internal sealed class OrderedIndexedDictionary<TKey, TValue> : IOrderedIndexedDictionary<TKey, TValue> where TKey : notnull
    {
        private readonly Action<TValue, int> IndexOrderedCollectionDelegate;
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
                    IndexOrderedCollection(collection.IndexOf(item));
                }
                else
                {
                    collection.Add(keyValuePair);
                    IndexOrderedCollectionDelegate(keyValuePair.Value, collection.Count - 1);
                }
            }
        }

        public TValue this[int index]
        {
            get => collection[index].Value;
            set
            {
                collection[index] = new(collection[index].Key, value);
                IndexOrderedCollection(index);
            }
        }

        public OrderedIndexedDictionary(Expression<Func<TValue, int>> indexedPropertyExpression)
        {
            IndexOrderedCollectionDelegate = GenerateDelegateToIndexItems<TValue>(indexedPropertyExpression);
            collection = new InternalKeyedCollection<TKey, KeyValuePair<TKey, TValue>>(item => item.Key);
        }

        public void Add(TKey key, TValue value)
        {
            collection.Add(new(key, value));
            IndexOrderedCollectionDelegate(value, collection.Count - 1);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            collection.Add(item);
            IndexOrderedCollectionDelegate(item.Value, collection.Count - 1);
        }

        public void Insert(int index, TKey key, TValue value)
        {
            collection.Insert(index, new(key, value));
            IndexOrderedCollection(index);
        }

        public void Clear() => collection.Clear();

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);

        public void RemoveAt(int index)
        {
            collection.RemoveAt(index);
            IndexOrderedCollection(index);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => collection.Contains(item);

        public bool ContainsKey(TKey key) => collection.Contains(key);

        public bool Remove(TKey key)
        {
            var index = collection.Select((item, index) => item.Key.Equals(key) ? index : -1).Max();
            var isRemoved = collection.Remove(key);
            IndexOrderedCollection(index);
            return isRemoved;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var index = collection.TakeWhile(k => !k.Key.Equals(item.Key)).Count() - 1;
            var isRemoved = collection.Remove(item);
            IndexOrderedCollection(index);
            return isRemoved;
        }

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

        private Action<TType, int> GenerateDelegateToIndexItems<TType>(Expression<Func<TType, int>> expression)
        {
            var valueParameter = Expression.Parameter(typeof(TType), "value");
            var indexParameter = Expression.Parameter(typeof(int), "valueParam");
            var property = Expression.Property(valueParameter, (expression.Body as MemberExpression).Member.Name);
            var valueProperty = Expression.Assign(property, indexParameter);
            var lambda = Expression.Lambda<Action<TType, int>>(valueProperty, valueParameter, indexParameter);
            return lambda.Compile();
        }

        public void IndexOrderedCollection(int startIndex)
        {
            for (int i = startIndex; i < collection.Count; i++)
            {
                IndexOrderedCollectionDelegate(collection[i].Value, i);
            }
        }
    }
}
