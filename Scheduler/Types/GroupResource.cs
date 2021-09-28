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
        private Guid id = Guid.NewGuid();
        private Visibility visibility = Visibility.Visible;
        public event PropertyChangedEventHandler PropertyChanged;

        public Guid Id => id;
        public IList<Appointment> Appointments { get; internal set; }

        public Visibility Visibility
        {
            get => visibility;
            internal set
            {
                visibility = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public abstract override string ToString();
        public abstract override int GetHashCode();
    }
}
