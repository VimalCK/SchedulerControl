using System;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Media;
using Scheduler.Common;
using static Scheduler.Common.Values;

namespace Scheduler
{
    internal sealed class TimeLineHeader : FrameworkElement
    {
        private double widthOfHour;
        private double averageHeight;
        private ScheduleControl parent;
        private DrawingGroup backingStore = new();

        public TimeLineHeader() => DefaultStyleKey = typeof(TimeLineHeader);

        ~TimeLineHeader() => parent.ScrollChanged -= ParentScrollChanged;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            parent = (ScheduleControl)TemplatedParent;
            parent.ScrollChanged += ParentScrollChanged;
        }

        private void ParentScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange is not Zero)
            {
                RenderContent(e.HorizontalOffset);
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (parent is null || ActualWidth is Zero)
            {
                return;
            }

            widthOfHour = parent.TestSize.Width / 24;
            averageHeight = ActualHeight / 3;
            drawingContext.DrawBorder(this, parent.TimeLineColor, BorderThickness);
            drawingContext.DrawDrawing(backingStore);

            RenderContent();
        }

        private void RenderContent(double horizontalOffset = 0)
        {
            var hoursScrolled = horizontalOffset / widthOfHour;
            var remainingWidthToScroll = (((int)hoursScrolled + 1) * widthOfHour) - horizontalOffset;
            var lineStartPoint = new Point(remainingWidthToScroll, 0);
            var lineEndPoint = new Point(remainingWidthToScroll, ActualHeight);
            var drawingContext = backingStore.Open();

            DrawTimeline(drawingContext, ref lineStartPoint, ref lineEndPoint, (int)hoursScrolled);
            drawingContext.DrawLine(new Pen(parent.TimeLineColor, HeaderLineThickness), lineStartPoint, lineEndPoint);

            if (parent.TimeLineZoom.Equals(TimeLineZoom.FortyEight))
            {
                lineStartPoint.X += widthOfHour;
                lineEndPoint.X += widthOfHour;
                DrawTimeline(drawingContext, ref lineStartPoint, ref lineEndPoint);
                drawingContext.DrawLine(new Pen(parent.TimeLineColor, HeaderLineThickness), lineStartPoint, lineEndPoint);
            }

            drawingContext.Close();
        }

        private void DrawTimeline(DrawingContext drawingContext,
            ref Point lineStartPoint,
            ref Point lineEndPoint,
            int startFrom = default)
        {
            //var clipWidth = Math.Max(0, lineStartPoint.X + timeLineGap - 3);
            for (int timeColumn = startFrom; timeColumn < 23; timeColumn++)
            {
                var formattedTime = TimeSpan.FromHours(timeColumn).ToString(TimeFormat);

                //drawingContext.PushClip(clipWidth, ActualHeight);
                // drawingContext.DrawText(this, formattedTime, lineStartPoint.X + TimeHeaderOffset, averageHeight);
                drawingContext.DrawLine(new Pen(parent.TimeLineColor, NarrowThickness), lineStartPoint, lineEndPoint);
                //drawingContext.Pop();
                lineStartPoint.X += widthOfHour;
                lineEndPoint.X = lineStartPoint.X;
                //clipWidth += timeLineGap;
            }
        }
    }
}
