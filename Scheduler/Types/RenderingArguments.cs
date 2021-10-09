using System;

namespace Scheduler.Types
{
    internal record RenderingArguments(DateTime SchedulerStartDate, DateTime SchedulerEndDate, int ExtendedMode, int TimelineZoom, double ViewPortAreaWidth);
}
