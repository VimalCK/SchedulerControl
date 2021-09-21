using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Scheduler
{
    internal sealed class TimeRulerPanel : FrameworkElement
    {
        private DispatcherTimer timer;
        private ScheduleControl parent;
        private DrawingGroup backingStore;

        public TimeRulerPanel()
        {
            backingStore = new DrawingGroup();
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
            Render();
            drawingContext.DrawDrawing(backingStore);
        }

        public void Render()
        {
            if (RenderRequired())
            {
                if (parent.ViewPortArea.Width > 0)
                {
                    var drawingContext = backingStore.Open();
                    var hourGap = parent.ViewPortArea.Width / (int)parent.TimeLineZoom;
                    var minuteGap = hourGap / 60;
                    var noOfDays = (DateTime.Now.Date - parent.StartDate.Date).Days;

                    var currentTimePosition = hourGap * (noOfDays * 24);
                    currentTimePosition += ((hourGap * DateTime.Now.Hour) + (minuteGap * DateTime.Now.Minute));

                    foreach (var ruler in parent.TimeLineProviders)
                    {
                        var xAxis = currentTimePosition + (hourGap * ruler.Hour) + (minuteGap * ruler.Minute);
                        if (xAxis > 0 && xAxis <= ActualWidth)
                        {
                            var pen = new Pen(ruler.Color, ruler.Thickness);
                            drawingContext.DrawLine(pen, new Point(xAxis, 0), new Point(xAxis, ActualHeight));
                        }
                    }

                    drawingContext.Close();
                }
            }
        }

        private void OnDispatcherCallbck(object sender, EventArgs e) => Render();

        private bool RenderRequired() => parent.StartDate != DateTime.MinValue;
    }
}
