using Scheduler.Types;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scheduler
{
    internal sealed class AppointmentRenderingCanvas : Canvas
    {
        private ScheduleControl parent;
        public AppointmentRenderingCanvas()
        {
            DefaultStyleKey = typeof(AppointmentRenderingCanvas);
            Appointment.AppointmentTimeChanged += OnAppointmentTimeChanged;
            Loaded += AppointmentRenderingCanvasLoaded;
        }

        ~AppointmentRenderingCanvas()
        {
            Appointment.AppointmentTimeChanged -= OnAppointmentTimeChanged;
            Loaded -= AppointmentRenderingCanvasLoaded;
        }

        protected override void OnInitialized(EventArgs e) => parent = this.GetParentOfType<ScheduleControl>();

        private void OnAppointmentTimeChanged(object sender, AppointmentTimeChangedEventArgs e)
        {

        }
        private void AppointmentRenderingCanvasLoaded(object sender, RoutedEventArgs e)
        {
            foreach (AppointmentItem appointment in Children)
            {
                RenderAppointment(appointment);
            }
        }
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            if (IsLoaded)
            {
                var appointment = (AppointmentItem)visualAdded;
                RenderAppointment(appointment);
            }
        }

        private void RenderAppointment(AppointmentItem appointment)
        {
            var dataContext = (Appointment)appointment.DataContext;

            if (dataContext.StartDateTime.Date >= parent.StartDate.Date && dataContext.EndDateTime.Date <= parent.EndDate.Date)
            {
                int timelineZoom = (int)parent.TimeLineZoom;
                var minuteGap = Math.Round((parent.ViewPortArea.Width / timelineZoom) / 60, 2);
                var left = (dataContext.StartDateTime - parent.StartDate.Date).TotalMinutes * minuteGap;
                var top = dataContext.Group.Order * timelineZoom;
                appointment.Height = (int)parent.ExtendedMode;
                appointment.Width = (dataContext.EndDateTime - dataContext.StartDateTime).TotalMinutes * minuteGap;
                Canvas.SetLeft(appointment, (int)left);
                Canvas.SetTop(appointment, (int)top);
            }
        }


    }
}
