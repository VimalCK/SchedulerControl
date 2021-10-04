using System;

namespace Scheduler.Types
{
    public delegate void AppointmentTimeChangedEventHandler(object sender, AppointmentTimeChangedEventArgs e);

    public sealed class AppointmentTimeChangedEventArgs : EventArgs
    {
        public DateTime OldValue { get; init; }
        public DateTime NewValue { get; init; }

        public AppointmentTimeChangedEventArgs(DateTime oldValue, DateTime newValue)
        => (OldValue, NewValue) = (oldValue, newValue);
    }
}
