using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using static Scheduler.Common.Values;

namespace Scheduler
{
    /// <summary>
    /// RulerGrid draws horizontal and vertical lines on the schedule control surface
    /// </summary>
    internal sealed class RulerGrid : FrameworkElement
    {
        private ScheduleControl parent;
        private DispatcherTimer timer;
        private readonly DrawingGroup backingStore = new();

        public RulerGrid()
        {
            DefaultStyleKey = typeof(RulerGrid);
            timer = new(new(0, 0, 0), DispatcherPriority.Normal, OnDispatcherCallbck, Dispatcher.CurrentDispatcher);

            timer.Interval = new(0, 1, 0);
            timer.Start();
        }

        ~ RulerGrid()
        {
            timer.Stop();
            timer = null;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            parent = TemplatedParent as ScheduleControl;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (parent is null || ActualWidth.Equals(Zero))
            {
                return;
            }

            Render();
            drawingContext.DrawDrawing(backingStore);
        }

        /// <summary>
        /// Rendering horizontal, vertical lines and Ruler lines.
        /// </summary>
        internal void Render()
        {
            var gapBetweenLines = (double)parent.ExtendedMode;
            var lineStartPoint = new Point(0, gapBetweenLines);
            var lineEndPoint = new Point(ActualWidth, gapBetweenLines);
            var pen = new Pen(parent.TimeLineColor, .5);
            var horizontalLinesRequired = parent.ViewPortArea.Width / (int)parent.ExtendedMode;

            using var drawingContext = backingStore.Open();
            while (horizontalLinesRequired > 0)
            {
                drawingContext.DrawLine(pen, lineStartPoint, lineEndPoint);
                lineStartPoint.Y += gapBetweenLines;
                lineEndPoint.Y = lineStartPoint.Y;
                horizontalLinesRequired--;
            }

            gapBetweenLines = parent.TestSize.Width / 24;
            lineStartPoint = new Point(gapBetweenLines, 0);
            lineEndPoint = new Point(gapBetweenLines, ActualHeight);
            var verticaLinesRequired = parent.ViewPortArea.Width / gapBetweenLines;

            while (verticaLinesRequired > 0)
            {
                drawingContext.DrawLine(pen, lineStartPoint, lineEndPoint);
                lineStartPoint.X += gapBetweenLines;
                lineEndPoint.X = lineStartPoint.X;
                verticaLinesRequired--;
            }

            if (!parent.TimeLineProviders.IsNullOrEmpty() && RenderRequired())
            {
                var minuteGap = gapBetweenLines / 60;
                var positionToDrawRuler = (DateTime.Now - parent.StartDate).TotalMinutes * minuteGap;
                foreach (var ruler in parent.TimeLineProviders)
                {
                    var xAxis = positionToDrawRuler + (ruler.Time.TotalMinutes * minuteGap);
                    if (xAxis > 0 && xAxis <= ActualWidth)
                    {
                        pen = new Pen(ruler.Color, ruler.Thickness);
                        drawingContext.DrawLine(pen, new Point(xAxis, 0), new Point(xAxis, ActualHeight));
                    }
                }
            }

            drawingContext.Close();
        }

        private void OnDispatcherCallbck(object sender, EventArgs e) => Render();

        private bool RenderRequired() => parent.StartDate != DateTime.MinValue;
    }
}
