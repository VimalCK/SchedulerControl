using Scheduler.Types;
using System.Windows;
using System.Windows.Controls;

namespace Scheduler
{
    internal sealed class AppointmentRenderingCanvas : Canvas
    {
        public AppointmentRenderingCanvas()
        {
            DefaultStyleKey = typeof(AppointmentRenderingCanvas);
            Appointment.GroupResourceChanged += Appointment_GroupResourceChanged;
        }

        private void Appointment_GroupResourceChanged(object sender, GroupResourceChangedEventArgs e)
        {
            
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }
    }
}
