using Scheduler.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scheduler
{
    internal class AppointmentContainer : ListBox
    {
        private ScheduleControl parent;
        public AppointmentContainer()
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
            //int timelineZoom = (int)parent.TimeLineZoom;
            //var minuteGap = (618 / timelineZoom);
            //base.PrepareContainerForItemOverride(element, item);
            //var control = element as Border;
            //Canvas.SetLeft(control, 0);
            //Canvas.SetTop(control, 0);
            //control.Height = 30;
            //control.Width = 2 * minuteGap;
            //control.Background = Brushes.Green;
        }
    }
}
