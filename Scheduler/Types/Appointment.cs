using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Scheduler.Types
{
    public class Appointment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        internal static event GroupResourceChangedEventHandler GroupResourceChanged;
        internal static event AppointmentTimeChangedEventHandler AppointmentTimeChanged;

        private DateTime startDateTime;
        private DateTime endDateTime;
        private string description;
        private GroupResource group;
        private double renderedHeight;
        private double renderedWidth;
        private Point located;
        private Visibility visibility = Visibility.Collapsed;

        public Visibility Visibility => visibility;

        public Point Located
        {
            get { return located; }
            internal set
            {
                located = value;
                OnPropertyChanged();
            }
        }

        public double RenderedWidth
        {
            get { return renderedWidth; }
            internal set
            {
                renderedWidth = value;
                OnPropertyChanged();
            }
        }

        public double RenderedHeight
        {
            get { return renderedHeight; }
            internal set
            {
                renderedHeight = value;
                OnPropertyChanged();
            }
        }

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

        public DateTime StartDateTime
        {
            get => startDateTime;
            set
            {
                var oldValue = startDateTime;
                startDateTime = value;
                OnPropertyChanged();
                if (oldValue != value)
                {
                    OnAppointmentDateChanged(oldValue, value);
                }
            }
        }

        public DateTime EndDateTime
        {
            get => endDateTime;
            set
            {
                var oldValue = endDateTime;
                endDateTime = value;
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

        public Appointment(DateTime startDateTime, DateTime endDateTime, GroupResource group) =>
            (this.startDateTime, this.endDateTime, this.group) = (startDateTime, endDateTime, group);

        internal void Show()
        {
            visibility = Visibility.Visible;
            OnPropertyChanged(nameof(Visibility));
        }

        internal void Hide()
        {
            visibility = Visibility.Collapsed;
            OnPropertyChanged(nameof(Visibility));
        }


        public override string ToString()
        {
            return Description;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void OnGroupResourceChanged(GroupResource oldValue, GroupResource newValue) =>
           GroupResourceChanged?.Invoke(this, new GroupResourceChangedEventArgs(oldValue, newValue));

        private void OnAppointmentDateChanged(DateTime oldValue, DateTime newValue) =>
            AppointmentTimeChanged?.Invoke(this, new AppointmentTimeChangedEventArgs(oldValue, newValue));
    }
}
