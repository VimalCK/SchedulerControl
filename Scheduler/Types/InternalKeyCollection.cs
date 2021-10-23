using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Scheduler.Types
{
    internal sealed class InternalKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem> where TKey : notnull
    {
        private readonly Func<TItem, TKey> getKeyForItemFunc;

        public InternalKeyedCollection(Func<TItem, TKey> getKeyForItemFunc) : this(getKeyForItemFunc, null) { }
        public InternalKeyedCollection(Func<TItem, TKey> getKeyForItemFunc, IEqualityComparer<TKey> comparer) : base(comparer)
        {
            if (getKeyForItemFunc is null)
            {
                throw new ArgumentNullException();
            }

            this.getKeyForItemFunc = getKeyForItemFunc;
        }

        protected override TKey GetKeyForItem(TItem item)
        {
            return getKeyForItemFunc(item);
        }

        public void Sort()
        {
            var list = base.Items as List<TItem>;
            list?.Sort(Comparer<TItem>.Default);
        }
    }
}
