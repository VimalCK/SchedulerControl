using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Scheduler.Types
{
    public class Appointment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        internal static event GroupResourceChangedEventHandler GroupResourceChanged;
        internal static event AppointmentTimeChangedEventHandler AppointmentTimeChanged;

        private DateTime startDate;
        private DateTime endDate;
        private string description;
        private GroupResource group;

        public GroupResource Group
        {
            get => group;
            set
            {
                var oldValue = group;
                group = value;
                OnPropertyChanged();
                if (oldValue != value)
                {
                    OnGroupResourceChanged(oldValue, group);
                }
            }
        }

        public DateTime StartDate
        {
            get => startDate;
            set
            {
                var oldValue = startDate;
                startDate = value;
                OnPropertyChanged();
                if (oldValue != value)
                {
                    OnAppointmentDateChanged(oldValue, value);
                }
            }
        }

        public DateTime EndDate
        {
            get => endDate;
            set
            {
                var oldValue = endDate;
                endDate = value;
                OnPropertyChanged();
                if (oldValue != value)
                {
                    OnAppointmentDateChanged(oldValue, value);
                }
            }
        }

        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }

        public Appointment(DateTime startDate, DateTime endDate, GroupResource group)
        {
            this.startDate = startDate;
            this.endDate = endDate;
            this.group = group;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void OnGroupResourceChanged(GroupResource oldValue, GroupResource newValue) =>
           GroupResourceChanged?.Invoke(this, new GroupResourceChangedEventArgs(oldValue, newValue));

        private void OnAppointmentDateChanged(DateTime oldValue, DateTime newValue) =>
            AppointmentTimeChanged?.Invoke(this, new AppointmentTimeChangedEventArgs(oldValue, newValue));
    }
}
