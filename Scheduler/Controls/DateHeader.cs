using Scheduler.Common;
using System;
using System.Diagnostics;
using System.Security.Cryptography.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Scheduler.Common.Values;

namespace Scheduler
{
    internal sealed class DateHeader : FrameworkElement
    {
        private double averageHeight;
        private DrawingGroup backingStore;
        private ScheduleControl parent;

        public DateHeader()
        {
            DefaultStyleKey = typeof(DateHeader);
            backingStore = new DrawingGroup();
            Loaded += DateHeaderLoaded;
        }

        ~DateHeader() => RemoveHandlers();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            parent = (ScheduleControl)TemplatedParent;
            parent.ScrollChanged += ScrollViewerScrollChanged;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (parent is null || ActualWidth == Zero)
            {
                return;
            }

            averageHeight = ActualHeight / 3;
            drawingContext.DrawBorder(this, parent.TimeLineColor, BorderThickness);
            drawingContext.DrawDrawing(backingStore);

            RenderContent(parent.HorizontalOffset);
        }

        internal void Render() => this.InvalidateVisual();

        private void RenderContent(double horizontalOffset = default)
        {
            var numberOfHeaders = 1;
            var difference = default(double);
            var timelineZoomWidth = parent.TestSize.Width;
            var numberOfHeadersScrolled = (int)(horizontalOffset / parent.TestSize.Width);
            if (parent.TimeLineZoom.Equals(TimeLineZoom.FortyEight))
            {
                numberOfHeaders = 2;
                difference = parent.TestSize.Width;
                timelineZoomWidth = parent.ViewPortArea.Width;
            }

            if (horizontalOffset is not Zero)
            {
                numberOfHeaders++;
                difference = timelineZoomWidth - (horizontalOffset % parent.TestSize.Width);
                Debug.WriteLine(horizontalOffset - (parent.TestSize.Width * numberOfHeadersScrolled));
                if (difference > timelineZoomWidth)
                {
                    return;
                }
            }

            var startPoint = new Point(difference, Zero);
            var endPoint = new Point(difference, ActualHeight);
            var drawingContext = backingStore.Open();

            for (int day = numberOfHeaders; day > Zero;)
            {
                var header = parent.StartDate.AddDays(numberOfHeadersScrolled + --day).ToString(ShortDateFormat);
                drawingContext.DrawLine(new Pen(parent.TimeLineColor, HeaderLineThickness), startPoint, endPoint);
                drawingContext.DrawText(this, header, endPoint.X + DateHeaderOffset, averageHeight);
                drawingContext.PushClip(Math.Max(0, startPoint.X), ActualHeight);
                startPoint.X = Math.Max(0, startPoint.X - parent.TestSize.Width);
                endPoint.X = startPoint.X;
            }

            drawingContext.Pop();
            drawingContext.Close();
        }

        private void RemoveHandlers() => parent.ScrollChanged -= ScrollViewerScrollChanged;
        private void DateHeaderLoaded(object sender, RoutedEventArgs e) => Loaded -= DateHeaderLoaded;
        private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e) => RenderContent(e.HorizontalOffset);
    }
}
