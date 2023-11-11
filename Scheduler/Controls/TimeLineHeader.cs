using System;
using System.Windows;
using Scheduler.Common;
using System.Windows.Media;
using static Scheduler.Common.Values;

namespace Scheduler
{
    /// <summary>
    /// TimeLineHeader controls responsible to show time and render below DateHeader
    /// </summary>
    internal sealed class TimeLineHeader : FrameworkElement
    {
        private readonly DrawingGroup backingStore = new();
        private ScheduleControl parent;

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

            drawingContext.DrawBorder(this, parent.TimeLineColor, new Thickness(BorderThickness));
            drawingContext.DrawDrawing(backingStore);

            RenderContent();
        }

        internal void Render() => this.InvalidateVisual();

        /// <summary>
        /// Rendering the timeline. Masking is applied not to overflow the text when control width shrinks 
        /// </summary>
        /// <param name="horizontalOffset"></param>
        internal void RenderContent(double horizontalOffset = 0)
        {
            var averageHeight = this.ActualHeight / 3;
            var timelineGap = parent.TestSize.Width / 24;
            var coulmnScrolled = (int)(horizontalOffset / timelineGap);
            var difference = horizontalOffset - (timelineGap * coulmnScrolled);
            var startLinePoint = new Point(timelineGap - difference, 0);
            var endLinePoint = new Point(startLinePoint.X, ActualHeight);
            var clipwidth = Math.Max(0, timelineGap - difference - TimeHeaderOffset);

            using var drawingContext = backingStore.Open();
            while (startLinePoint.X - timelineGap < parent.ViewPortArea.Width)
            {
                var header = TimeSpan.FromHours(coulmnScrolled++);
                drawingContext.DrawLine(new Pen(parent.TimeLineColor, HeaderLineThickness), startLinePoint, endLinePoint);
                drawingContext.PushClip(clipwidth, ActualHeight);
                drawingContext.DrawText(this, header.ToString(TimeFormat), startLinePoint.X - timelineGap + TimeHeaderOffset, averageHeight);
                startLinePoint.X += timelineGap;
                endLinePoint.X = startLinePoint.X;
                clipwidth += timelineGap;
                drawingContext.Pop();
            }

            drawingContext.Close();
        }

        /// <summary>
        /// Redrawing while scrolling Schedule control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParentScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange is not Zero)
            {
                RenderContent(e.HorizontalOffset);
            }
        }
    }
}
