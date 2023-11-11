using System;
using System.Windows;
using Scheduler.Common;
using System.Windows.Media;
using System.Windows.Controls;
using static Scheduler.Common.Values;

namespace Scheduler
{
    /// <summary>
    /// DateHeader is responsible to draw dates on top of the schedule control. 
    /// </summary>
    internal sealed class DateHeader : FrameworkElement
    {
        private readonly DrawingGroup backingStore;
        private double averageHeight;
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
            drawingContext.DrawBorder(this, parent.TimeLineColor, new Thickness(BorderThickness) { Bottom = 0 });
            drawingContext.DrawDrawing(backingStore);

            RenderContent(parent.HorizontalOffset);
        }

        internal void Render() => this.InvalidateVisual();

        /// <summary>
        /// Rendering the Number of dates to show. DateHeader draws first date on the left most side 
        /// and adjust remining content based on scrolling position
        /// </summary>
        /// <param name="horizontalOffset"></param>
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
                if (difference > timelineZoomWidth)
                {
                    return;
                }
            }

            var startPoint = new Point(difference, Zero);
            var endPoint = new Point(difference, ActualHeight);

            using var drawingContext = backingStore.Open();
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

        /// <summary>
        /// Redraw the control while schedule control is scrolled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange is not Zero)
            {
                RenderContent(e.HorizontalOffset);
            }
        }
    }
}