using System;
using System.Windows;
using System.Windows.Media;
using Scheduler.Common;
using static Scheduler.Common.Values;

namespace Scheduler
{
    internal sealed class TimeLineHeader : FrameworkElement
    {
        private ScheduleControl parent;
        private readonly DrawingGroup backingStore = new();

        public TimeLineHeader() => DefaultStyleKey = typeof(TimeLineHeader);

        ~TimeLineHeader() => parent.ScrollChanged -= ParentScrollChanged;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            parent = (ScheduleControl)TemplatedParent;
            parent.ScrollChanged += ParentScrollChanged;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (parent is null || ActualWidth is Zero)
            {
                return;
            }

            drawingContext.DrawBorder(this, parent.TimeLineColor, BorderThickness);
            drawingContext.DrawDrawing(backingStore);

            RenderContent();
        }

        internal void RenderContent(double horizontalOffset = 0)
        {
            var averageHeight = this.ActualHeight / 3;
            var timelineGapWidth = parent.TestSize.Width / 24;
            var coulmnScrolled = (int)(horizontalOffset / timelineGapWidth);
            var difference = horizontalOffset - (timelineGapWidth * coulmnScrolled);
            var startLinePoint = new Point(timelineGapWidth - difference, 0);
            var endLinePoint = new Point(startLinePoint.X, ActualHeight);
            var clipwidth = Math.Max(0, timelineGapWidth - difference - TimeHeaderOffset);

            using var drawingContext = backingStore.Open();
            while (startLinePoint.X - timelineGapWidth < parent.ViewPortArea.Width)
            {
                var header = TimeSpan.FromHours(coulmnScrolled++);
                var timelineColor = header.Hours.Equals(TwentyThree) ? HeaderLineThickness : NarrowThickness;
                drawingContext.DrawLine(new Pen(parent.TimeLineColor, timelineColor), startLinePoint, endLinePoint);
                drawingContext.PushClip(clipwidth, ActualHeight);
                drawingContext.DrawText(this, header.ToString(TimeFormat), startLinePoint.X - timelineGapWidth + TimeHeaderOffset, averageHeight);
                startLinePoint.X += timelineGapWidth;
                endLinePoint.X = startLinePoint.X;
                clipwidth += timelineGapWidth;
                drawingContext.Pop();
            }
        }

        private void ParentScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange is not Zero)
            {
                RenderContent(e.HorizontalOffset);
            }
        }
    }
}
