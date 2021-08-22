using System;

namespace Scheduler
{
    public interface IAppointment
    {
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        string Description { get; set; }
        string Group { get; set; }
    }
}
