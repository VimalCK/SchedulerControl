using System;
using System.Windows;
using System.Windows.Controls;

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
    }
}
