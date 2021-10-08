using Scheduler.Types;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Scheduler
{
    internal sealed class AppointmentRenderingCanvas : Canvas
    {
        private ScheduleControl parent;
        private bool inPorgress = false;
        private readonly DispatcherTimer dispatcherTimer = new();
        private Queue<AppointmentItem> queue = new();

        public AppointmentRenderingCanvas()
        {
            DefaultStyleKey = typeof(AppointmentRenderingCanvas);
            Appointment.AppointmentTimeChanged += OnAppointmentTimeChanged;
        }

        ~AppointmentRenderingCanvas()
        {
            Appointment.AppointmentTimeChanged -= OnAppointmentTimeChanged;
        }

        internal void Process(IEnumerable<AppointmentItem> appointments, ScheduleControl appointmentContainer)
        {
            if (!appointments.IsNullOrEmpty())
            {
                foreach (var item in appointments)
                {
                    queue.Enqueue(item);
                }

                if (!inPorgress)
                {
                    dispatcherTimer.Tick += DispatcherTimerTick;
                    dispatcherTimer.Start();
                }
            }
        }

        protected override void OnInitialized(EventArgs e) => parent = this.GetParentOfType<ScheduleControl>();
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            if (IsLoaded)
            {
                var appointment = (AppointmentItem)visualAdded;
                Process(new List<AppointmentItem>() { appointment }, parent);
            }
        }

        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            dispatcherTimer.Tick -= DispatcherTimerTick;
            dispatcherTimer.Stop();
            inPorgress = true;

            while (queue.TryDequeue(out AppointmentItem appointment))
            {
                var dataContext = (Appointment)appointment.DataContext;
                if (dataContext.StartDateTime.Date >= parent.StartDate.Date &&
                    dataContext.EndDateTime.Date <= parent.EndDate.Date)
                {
                    int timelineZoom = (int)parent.TimeLineZoom;
                    var minuteGap = (parent.ViewPortArea.Width / timelineZoom) / 60;
                    appointment.Height = (int)parent.ExtendedMode;
                    appointment.Width = (dataContext.EndDateTime - dataContext.StartDateTime).TotalMinutes * minuteGap;
                    Canvas.SetLeft(appointment, (dataContext.StartDateTime - parent.StartDate.Date).TotalMinutes * minuteGap);
                    Canvas.SetTop(appointment, appointment.Height * dataContext.Group.Order);
                }
            }

            inPorgress = false;
        }

        private void OnAppointmentTimeChanged(object sender, AppointmentTimeChangedEventArgs e)
        {

        }
    }
}
