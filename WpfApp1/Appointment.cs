﻿using Scheduler;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    public class Appointment : IAppointment
    {
        private DateTime startDate;
        private DateTime endDate;
        private string description;
        private string group;
        public string Group
        {
            get => group;
            set => group = value;
        }
        public DateTime StartDate
        {
            get => startDate;
            set => startDate = value;
        }

        public DateTime EndDate
        {
            get => endDate;
            set => endDate = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }
    }
}
