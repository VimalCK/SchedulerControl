using Scheduler.Common;
using System;
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

        internal void Render() => this.InvalidateVisual();
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

        private void RenderContent(double horizontalOffset = default)
        {
            var difference = horizontalOffset % parent.TestSize.Width;
            var numberOfHeadersScrolled = (int)(horizontalOffset / parent.TestSize.Width);
            var numberOfHeaders = parent.TimeLineZoom.Equals(TimeLineZoom.FortyEight) ? 2 : 1;
            difference = horizontalOffset.Equals(Zero) ? parent.TestSize.Width : parent.ViewPortArea.Width - difference;
            if (horizontalOffset is not Zero)
            {
                numberOfHeaders++;
            }

            var startPoint = new Point(difference, 0);
            var endPoint = new Point(difference, ActualHeight);
            var drawingContext = backingStore.Open();


            for (int day = numberOfHeaders; day > Zero;)
            {
                var header = parent.StartDate.AddDays(numberOfHeadersScrolled + --day).ToString(ShortDateFormat);
                drawingContext.DrawLine(new Pen(parent.TimeLineColor, HeaderLineThickness), startPoint, endPoint);
                drawingContext.DrawText(this, header, endPoint.X + DateHeaderOffset, averageHeight);
                startPoint.X -= parent.TestSize.Width;
                endPoint.X = startPoint.X;
            }

            drawingContext.Close();
            //var dayOffset = (int)(horizontalOffset / templatedParent.TestSize.Width);
            //var requiredHeaderWidth = ((dayOffset + 1) * templatedParent.TestSize.Width) - horizontalOffset;
            //var startPoint = new Point(requiredHeaderWidth, Zero);
            //var endPoint = new Point(requiredHeaderWidth, ActualHeight);
            //var header = templatedParent.StartDate.AddDays(dayOffset + 1).ToString(ShortDateFormat);
            //var drawingContext = backingStore.Open();

            //drawingContext.DrawLine(new Pen(templatedParent.TimeLineColor, HeaderLineThickness), startPoint, endPoint);
            //drawingContext.DrawText(this, header, endPoint.X + DateHeaderOffset, averageHeight);

            //if (templatedParent.TimeLineZoom.Equals(TimeLineZoom.FortyEight))
            //{
            //    var nearAdjacentHeaderWidth = requiredHeaderWidth + templatedParent.TestSize.Width;
            //    startPoint = new Point(nearAdjacentHeaderWidth, Zero);
            //    endPoint = new Point(nearAdjacentHeaderWidth, ActualHeight);
            //    header = templatedParent.StartDate.AddDays(dayOffset + 2).ToString(ShortDateFormat);
            //    drawingContext.DrawLine(new Pen(templatedParent.TimeLineColor, HeaderLineThickness), startPoint, endPoint);
            //    drawingContext.DrawText(this, header, endPoint.X + DateHeaderOffset, averageHeight);
            //}

            //header = templatedParent.StartDate.AddDays(dayOffset).ToString(ShortDateFormat);
            //drawingContext.PushClip(requiredHeaderWidth, ActualHeight);
            //drawingContext.DrawText(this, header, DateHeaderOffset, averageHeight);
            //drawingContext.Pop();
            //drawingContext.Close();
        }

        private void RemoveHandlers() => parent.ScrollChanged -= ScrollViewerScrollChanged;
        private void DateHeaderLoaded(object sender, RoutedEventArgs e) => Loaded -= DateHeaderLoaded;
        private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e) => RenderContent(e.HorizontalOffset);
    }
}
