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
        public AppointmentContainer() => this.DefaultStyleKey = typeof(ListBox);

        public override void OnApplyTemplate()
        {
            var border = (Border)GetVisualChild(0);
            border.Padding = new(0);

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            parent = (ScheduleControl)TemplatedParent;
        }

        protected override DependencyObject GetContainerForItemOverride() => new AppointmentItem();
        protected override bool IsItemItsOwnContainerOverride(object item) => item is AppointmentItem;
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
