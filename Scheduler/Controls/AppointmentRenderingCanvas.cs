using Scheduler.Types;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Scheduler
{
    internal sealed class AppointmentRenderingCanvas : Canvas
    {
        private ScheduleControl parent;
        public AppointmentRenderingCanvas()
        {
            DefaultStyleKey = typeof(AppointmentRenderingCanvas);
            Appointment.AppointmentTimeChanged += OnAppointmentTimeChanged;
        }

        ~AppointmentRenderingCanvas()
        {
            Appointment.AppointmentTimeChanged -= OnAppointmentTimeChanged;
        }

        protected override void OnInitialized(EventArgs e) => parent = this.GetParentOfType<ScheduleControl>();

        private void OnAppointmentTimeChanged(object sender, AppointmentTimeChangedEventArgs e)
        {

        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            var appointment = (AppointmentItem)visualAdded;
            var dataContext = (Appointment)appointment.DataContext;

            if (dataContext.StartDateTime.Date >= parent.StartDate.Date && dataContext.EndDateTime.Date <= parent.EndDate.Date)
            {
                var minuteGap = (parent.ViewPortArea.Width / (int)parent.TimeLineZoom) / 60;
                var left = (dataContext.StartDateTime - parent.StartDate.Date).TotalMinutes * minuteGap;
                var width = (dataContext.EndDateTime - dataContext.StartDateTime).TotalMinutes * minuteGap;
               
            }
        }
    }
}
