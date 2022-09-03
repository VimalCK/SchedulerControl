using Scheduler.Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Scheduler.Common.Values;

namespace Scheduler
{
    internal sealed class DateHeader : FrameworkElement
    {
        private DrawingGroup backingStore;
        private ScheduleControl templatedParent;

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
            templatedParent = (ScheduleControl)TemplatedParent;
            templatedParent.ScrollChanged += ScrollViewerScrollChanged;
        }

        internal void Render() => this.InvalidateVisual();
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (templatedParent is null || ActualWidth == Zero)
            {
                return;
            }

            drawingContext.DrawBorder(this, templatedParent.TimeLineColor, BorderThickness);
            drawingContext.DrawDrawing(backingStore);

            RenderContent(templatedParent.HorizontalOffset);
        }

        private void RenderContent(double horizontalOffset = default)
        {
            var averageHeight = ActualHeight / 3;
            var dayOffset = (int)(horizontalOffset / templatedParent.TestSize.Width);
            var requiredHeaderWidth = ((dayOffset + 1) * templatedParent.TestSize.Width) - horizontalOffset;
            var startPoint = new Point(requiredHeaderWidth, Zero);
            var endPoint = new Point(requiredHeaderWidth, ActualHeight);
            var header = templatedParent.StartDate.AddDays(dayOffset).ToString(ShortDateFormat);
            var drawingContext = backingStore.Open();

            if (templatedParent.TimeLineZoom.Equals(TimeLineZoom.FortyEight))
            {
                var adjacentHeader = templatedParent.StartDate.AddDays(++dayOffset).ToString(ShortDateFormat);
                drawingContext.DrawLine(new Pen(templatedParent.TimeLineColor, HeaderLineThickness), startPoint, endPoint);
                drawingContext.DrawText(this, adjacentHeader, endPoint.X + DateHeaderOffset, averageHeight);
            }

            if (horizontalOffset is not Zero)
            {
                var nearAdjacentHeader = templatedParent.StartDate.AddDays(++dayOffset).ToString(ShortDateFormat);
                var nearAdjacentHeaderWidth = requiredHeaderWidth + templatedParent.TestSize.Width;
                startPoint = new Point(nearAdjacentHeaderWidth, Zero);
                endPoint = new Point(nearAdjacentHeaderWidth, ActualHeight);
                drawingContext.DrawLine(new Pen(templatedParent.TimeLineColor, HeaderLineThickness), startPoint, endPoint);
                drawingContext.DrawText(this, nearAdjacentHeader, endPoint.X + DateHeaderOffset, averageHeight);
            }

            drawingContext.PushClip(requiredHeaderWidth, ActualHeight);
            drawingContext.DrawText(this, header, DateHeaderOffset, averageHeight);
            drawingContext.Pop();
            drawingContext.Close();
        }

        private void RemoveHandlers() => templatedParent.ScrollChanged -= ScrollViewerScrollChanged;
        private void DateHeaderLoaded(object sender, RoutedEventArgs e) => Loaded -= DateHeaderLoaded;
        private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e) => RenderContent(e.HorizontalOffset);
    }
}
