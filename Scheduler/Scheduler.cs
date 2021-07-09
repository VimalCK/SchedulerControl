using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scheduler
{
    internal class Scheduler : ListBox
    {
        private ScheduleControl parent;
        public Scheduler()
        {
            this.DefaultStyleKey = typeof(ListBox);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            parent = (ScheduleControl)TemplatedParent;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new AppointmentItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is AppointmentItem;
        }
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element is AppointmentItem appointmentItem)
            {
                appointmentItem.Height = parent.ExtendedModeSize - 5;
                appointmentItem.Width = 100;
                appointmentItem.Background = Brushes.Red;
            }
        }
    }
}
