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

        internal void Render(IEnumerable<Appointment> appointments)
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
                foreach (var appointment in appointments)
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
                }
            }
        }

        internal void MeasureWidth(IEnumerable<Appointment> appointments)
        {
            if (!appointments.IsNullOrEmpty() && !parent.GroupBy.IsNullOrEmpty())
            {
                RenderingArguments arg = new
                    (
                        SchedulerStartDate: parent.StartDate,
                        ExtendedMode: (int)parent.ExtendedMode
                    );

                var minuteGap = (parent.ViewPortArea.Width / (int)parent.TimeLineZoom) / 60;
                foreach (var appointment in appointments)
                {
                    appointment.RenderedWidth = (appointment.EndDateTime - appointment.StartDateTime).TotalMinutes * minuteGap;
                    appointment.Located = new Point
                    {
                        X = (appointment.StartDateTime - arg.SchedulerStartDate.Date).TotalMinutes * minuteGap,
                        Y = arg.ExtendedMode * appointment.Group.Order
                    };
                }
            }
        }

        internal void MeasureHeight(IEnumerable<Appointment> appointments)
        {
            if (!appointments.IsNullOrEmpty() && !parent.GroupBy.IsNullOrEmpty())
            {
                RenderingArguments arg = new
                    (
                        SchedulerStartDate: parent.StartDate,
                        ExtendedMode: (int)parent.ExtendedMode
                    );

                var minuteGap = (parent.ViewPortArea.Width / (int)parent.TimeLineZoom) / 60;
                foreach (var appointment in appointments)
                {
                    appointment.RenderedHeight = arg.ExtendedMode / 2;
                    appointment.Located = new Point
                    {
                        X = (appointment.StartDateTime - arg.SchedulerStartDate.Date).TotalMinutes * minuteGap,
                        Y = arg.ExtendedMode * appointment.Group.Order
                    };
                }
            }
        }

        protected override void OnInitialized(EventArgs e) => parent = this.GetParentOfType<ScheduleControl>();

        private void OnAppointmentTimeChanged(object sender, AppointmentTimeChangedEventArgs e)
        {
            Render(new[] { (Appointment)sender });
        }
    }
}
