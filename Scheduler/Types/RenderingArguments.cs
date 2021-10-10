using System;

namespace Scheduler.Types
{
    internal record RenderingArguments(DateTime SchedulerStartDate = default, DateTime SchedulerEndDate = default, int ExtendedMode = default);
}
