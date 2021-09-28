﻿using Scheduler;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp1
{
    public class Appointment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime startDate;
        private DateTime endDate;
        private string description;
        private GroupResource group;

        public GroupResource Group
        {
            get => group;
            set
            {
                group = value;
                OnPropertyChanged();
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

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
