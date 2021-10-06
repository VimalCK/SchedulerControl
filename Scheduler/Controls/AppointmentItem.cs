using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scheduler
{
    internal sealed class AppointmentItem : Border
    {
        public AppointmentItem()
        {
            DefaultStyleKey = typeof(AppointmentItem);
        }
    }
}
