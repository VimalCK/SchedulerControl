using System;

namespace Scheduler.Types
{
    public delegate void GroupResourceChangedEventHandler(object sender, GroupResourceChangedEventArgs e);

    public sealed class GroupResourceChangedEventArgs : EventArgs
    {
        public GroupResource OldValue { get; init; }
        public GroupResource NewValue { get; init; }

        public GroupResourceChangedEventArgs(GroupResource oldValue, GroupResource newValue)
            => (OldValue, NewValue) = (oldValue, newValue);
    }
}