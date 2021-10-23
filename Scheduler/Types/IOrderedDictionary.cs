using System.Collections.Generic;

namespace Scheduler.Types
{
    internal interface IOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
    {
        void Insert(int index, TKey key, TValue value);
        void RemoveAt(int index);
    }
}
