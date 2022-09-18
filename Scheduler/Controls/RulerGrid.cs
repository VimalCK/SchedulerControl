using System;
using System.Windows;
using System.Windows.Media;
using static Scheduler.Common.Values;

namespace Scheduler
{
    internal sealed class RulerGrid : FrameworkElement
    {
        private ScheduleControl parent;
        private readonly DrawingGroup backingStore = new();

        public RulerGrid() => DefaultStyleKey = typeof(RulerGrid);

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

        internal void Render()
        {
            var gap = (double)parent.ExtendedMode;
            var startPoint = new Point(0, gap);
            var endPoint = new Point(ActualWidth, gap);
            var pen = new Pen(parent.TimeLineColor, .5);
            var numberOfLinesRequired = parent.ViewPortArea.Width / (int)parent.ExtendedMode;

            using var drawingContext = backingStore.Open();
            while (numberOfLinesRequired > 0)
            {
                drawingContext.DrawLine(pen, startPoint, endPoint);
                startPoint.Y += gap;
                endPoint.Y = startPoint.Y;
                numberOfLinesRequired--;
            }

            gap = parent.TestSize.Width / 24;
            startPoint = new Point(gap, 0);
            endPoint = new Point(gap, ActualHeight);
            numberOfLinesRequired = parent.ViewPortArea.Width / gap;

            while (numberOfLinesRequired > 0)
            {
                drawingContext.DrawLine(pen, startPoint, endPoint);
                startPoint.X += gap;
                endPoint.X = startPoint.X;
                numberOfLinesRequired--;
            }
        }
    }
}
