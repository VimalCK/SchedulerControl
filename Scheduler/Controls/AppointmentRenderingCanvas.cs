using Scheduler.Types;
using System;
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

        internal async ValueTask RenderAsync(params Appointment[] appointments)
        {
            if (!appointments.IsNullOrEmpty())
            {
                RenderingArguments arg = new(parent.StartDate, parent.EndDate, (int)parent.ExtendedMode, (int)parent.TimeLineZoom, parent.ViewPortArea.Width);
                await Parallel.ForEachAsync(appointments, (appointment, token) =>
                {
                    var minuteGap = (arg.ViewPortAreaWidth / arg.TimelineZoom) / 60;
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
                    }

                    return ValueTask.CompletedTask;
                });
            }
        }

        protected override void OnInitialized(EventArgs e) => parent = this.GetParentOfType<ScheduleControl>();

        private void OnAppointmentTimeChanged(object sender, AppointmentTimeChangedEventArgs e)
        {

        }
    }
}
