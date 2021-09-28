using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Scheduler.Types
{
    public class Appointment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public static event GroupResourceChangedEventHandler GroupResourceChanged;

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
                OnGroupResourceChanged(oldValue, group);
            }
        }

        public DateTime StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                OnPropertyChanged();
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

        public void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void OnGroupResourceChanged(GroupResource oldValue, GroupResource newValue) =>
           GroupResourceChanged?.Invoke(this, new GroupResourceChangedEventArgs(oldValue, newValue));

    }
}
