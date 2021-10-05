using Scheduler.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Scheduler
{
    public abstract class GroupResource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Guid id = Guid.NewGuid();
        private Visibility visibility = Visibility.Visible;

        public Guid Id => id;
        public IList<Appointment> Appointments { get; internal set; } = new List<Appointment>();

        public Visibility Visibility
        {
            get => visibility;
            internal set
            {
                visibility = value;
                OnPropertyChanged();
            }
        }

        internal int Order { get; set; }

        public abstract override string ToString();
        public abstract override int GetHashCode();
        public void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
