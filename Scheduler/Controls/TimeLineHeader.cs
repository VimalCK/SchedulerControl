using Scheduler.Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Scheduler.UIExtensions;
using static Scheduler.Common.Values;

namespace Scheduler
{
    internal sealed class TimeLineHeader : FrameworkElement
    {
        private ScheduleControl parent;
        //private TranslateTransform transform = new();
        private DrawingGroup backingStore = new();
        public TimeLineHeader()
        {
            DefaultStyleKey = typeof(TimeLineHeader);
            //this.RenderTransform = transform;
        }

        ~TimeLineHeader() => parent.ScrollChanged -= ParentScrollChanged;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            parent = (ScheduleControl)TemplatedParent;
            parent.ScrollChanged += ParentScrollChanged;
        }

        private void ParentScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange != Zero)
            {
                //transform.X -= e.HorizontalChange;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (parent is null || ActualWidth == Zero)
            {
                return;
            }

            drawingContext.DrawBorder(this, parent.TimeLineColor, BorderThickness);
            drawingContext.DrawDrawing(backingStore);

            RenderContent();

        }

        private void RenderContent()
        {
            var headerText = Zero;
            var averageHeight = ActualHeight / 3;
            var lineStartPoint = new Point();
            var lineEndPoint = new Point(0, ActualHeight);
            var timeLineGap = parent.TestSize.Width / 24;
            var clipWidth = Math.Max(0, timeLineGap - 3);
            var drawingContext = backingStore.Open();

            for (int timeColumn = 0; timeColumn < 24; timeColumn++)
            {
                var formattedTime = TimeSpan.FromHours(headerText++).ToString(TimeFormat);

                drawingContext.PushClip(clipWidth, ActualHeight);
                drawingContext.DrawText(this, formattedTime, lineStartPoint.X + TimeHeaderOffset, averageHeight);
                drawingContext.DrawLine(new Pen(parent.TimeLineColor, NarrowThickness), lineStartPoint, lineEndPoint);
                drawingContext.Pop();
                lineStartPoint.X += timeLineGap;
                lineEndPoint.X = lineStartPoint.X;
                clipWidth += timeLineGap;
            }
               
            drawingContext.DrawLine(new Pen(parent.TimeLineColor, HeaderLineThickness), lineStartPoint, lineEndPoint);

            drawingContext.Close();
        }
    }
}
