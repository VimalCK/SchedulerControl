using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler
{
    public interface IAppointment
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
    }
}
