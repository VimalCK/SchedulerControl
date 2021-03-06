using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Scheduler
{
    [TemplatePart(Name = "PART_SchedulerCanvas", Type = typeof(Canvas))]
    internal sealed class Scheduler : ItemsControl
    {
        public Scheduler() => this.DefaultStyleKey = typeof(Scheduler);
    }
}
