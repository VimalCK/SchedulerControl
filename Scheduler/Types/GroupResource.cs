using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Scheduler
{
    public abstract class GroupResource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Visibility visibility = Visibility.Visible;


        public IList<IAppointment> Appointments { get; internal set; }

        public Visibility Visibility
        {
            get => visibility;
            internal set
            {
                visibility = value;
                OnPropertyChanged();
            }
        }

        public abstract override string ToString();
        public abstract override int GetHashCode();

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
