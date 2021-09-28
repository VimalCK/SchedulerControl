using System;

namespace Scheduler.Types
{
    public delegate void GroupResourceChangedEventHandler(object sender, GroupResourceChangedEventArgs e);

    public sealed class GroupResourceChangedEventArgs : EventArgs
    {
        private readonly GroupResource oldValue;
        private readonly GroupResource newValue;

        public GroupResource OldValue => oldValue;
        public GroupResource NewValue => newValue;

        public GroupResourceChangedEventArgs(GroupResource oldValue, GroupResource newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}