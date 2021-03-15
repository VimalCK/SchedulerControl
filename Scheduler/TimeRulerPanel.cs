using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Scheduler
{
    internal sealed class TimeRulerPanel : FrameworkElement
    {
        private DispatcherTimer timer;
        private ScheduleControl parent;
        //internal ObservableCollection<TimeRuler> TimeRulers { get; set; } = new ObservableCollection<TimeRuler>();

        public TimeRulerPanel()
        {
            timer = new DispatcherTimer(new TimeSpan(0, 0, 0), DispatcherPriority.Normal,
                OnDispatcherCallbck, Dispatcher.CurrentDispatcher);

            timer.Interval = new TimeSpan(0, 1, 0);
            timer.Start();
        }
        ~TimeRulerPanel()
        {
            timer.Stop();
            timer = null;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            parent = (ScheduleControl)TemplatedParent;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (RenderRequired())
            {
                var verticalGap = ActualWidth / ((int)parent.TimeLineZoom * parent.ViewRange);
                var currentPosition = verticalGap * (((DateTime.Now - parent.StartDate).Days * 24) + DateTime.Now.TimeOfDay.Hours);
                var minuteGap = verticalGap / 60;

                currentPosition += DateTime.Now.TimeOfDay.Minutes * minuteGap;

                foreach (var ruler in parent.TimeLineProviders)
                {
                    if (!string.IsNullOrEmpty(ruler.Time))
                    {
                        var time = ruler.Time.Split(':');

                        if (time.Length == 0)
                        {
                            throw new Exception();
                        }

                        currentPosition += (verticalGap * int.Parse(time[0])) + (int.Parse(time[1]) * minuteGap);
                    }

                    var pen = new Pen(ruler.Color, 2);

                    drawingContext.DrawLine(pen, new Point(currentPosition, 0), new Point(currentPosition, ActualHeight));
                }
            }

        }

        private void OnDispatcherCallbck(object sender, EventArgs e) => InvalidateVisual();

        private bool RenderRequired() => parent.StartDate != DateTime.MinValue && parent.StartDate <= DateTime.Now;
    }
}
