using Scheduler.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        internal async ValueTask RenderAsync(IEnumerable<Appointment> appointments)
        {
            if (!appointments.IsNullOrEmpty() && !parent.GroupBy.IsNullOrEmpty())
            {
                RenderingArguments arg = new
                    (
                        parent.StartDate,
                        parent.EndDate,
                        (int)parent.ExtendedMode
                    );

                var minuteGap = (parent.ViewPortArea.Width / (int)parent.TimeLineZoom) / 60;
                await Parallel.ForEachAsync(appointments, (appointment, token) =>
                {
                    if (appointment.StartDateTime.Date >= arg.SchedulerStartDate.Date &&
                        appointment.EndDateTime.Date <= arg.SchedulerEndDate.Date)
                    {
                        appointment.RenderedHeight = arg.ExtendedMode / 2;
                        appointment.RenderedWidth = (appointment.EndDateTime - appointment.StartDateTime).TotalMinutes * minuteGap;
                        appointment.Located = new Point
                        {
                            X = (appointment.StartDateTime - arg.SchedulerStartDate.Date).TotalMinutes * minuteGap,
                            Y = arg.ExtendedMode * appointment.Group.Order
                        };

                        appointment.Show();
                    }
                    else
                    {
                        appointment.Hide();
                    }

                    return ValueTask.CompletedTask;
                });
            }
        }

        internal async ValueTask MeasureWidthAsync(IEnumerable<Appointment> appointments)
        {
            if (!appointments.IsNullOrEmpty() && !parent.GroupBy.IsNullOrEmpty())
            {
                RenderingArguments arg = new
                    (
                        SchedulerStartDate: parent.StartDate,
                        ExtendedMode: (int)parent.ExtendedMode
                    );

                var minuteGap = (parent.ViewPortArea.Width / (int)parent.TimeLineZoom) / 60;
                await Parallel.ForEachAsync(appointments, (appointment, token) =>
                {
                    appointment.RenderedWidth = (appointment.EndDateTime - appointment.StartDateTime).TotalMinutes * minuteGap;
                    appointment.Located = new Point
                    {
                        X = (appointment.StartDateTime - arg.SchedulerStartDate.Date).TotalMinutes * minuteGap,
                        Y = arg.ExtendedMode * appointment.Group.Order
                    };

                    return ValueTask.CompletedTask;
                });
            }
        }

        internal async ValueTask MeasureHeightAsync(IEnumerable<Appointment> appointments)
        {
            if (!appointments.IsNullOrEmpty() && !parent.GroupBy.IsNullOrEmpty())
            {
                RenderingArguments arg = new
                    (
                        SchedulerStartDate: parent.StartDate,
                        ExtendedMode: (int)parent.ExtendedMode
                    );

                var minuteGap = (parent.ViewPortArea.Width / (int)parent.TimeLineZoom) / 60;
                await Parallel.ForEachAsync(appointments, (appointment, token) =>
                {
                    appointment.RenderedHeight = arg.ExtendedMode / 2;
                    appointment.Located = new Point
                    {
                        X = (appointment.StartDateTime - arg.SchedulerStartDate.Date).TotalMinutes * minuteGap,
                        Y = arg.ExtendedMode * appointment.Group.Order
                    };

                    return ValueTask.CompletedTask;
                });
            }
        }

        protected override void OnInitialized(EventArgs e) => parent = this.GetParentOfType<ScheduleControl>();

        private async void OnAppointmentTimeChanged(object sender, AppointmentTimeChangedEventArgs e)
        {
            await RenderAsync(new[] { (Appointment)sender });
        }
    }
}
