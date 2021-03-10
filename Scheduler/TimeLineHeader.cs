using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Scheduler
{
    internal sealed class TimeLineHeader : RulerBase
    {
        private CultureInfo cultureInfo = new CultureInfo("en-US");
        private Typeface typeface = new Typeface("Arial");

        public TimeLineHeader() => DefaultStyleKey = typeof(TimeLineHeader);

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.TemplatedParent is ScheduleControl control && control.ActualWidth != 0)
            {
                var renderPoint = new Point(0, this.ActualHeight / 3);
                var pixelPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
                var timeLineValue = (int)control.TimeLineZoom;

                this.VerticalLines = timeLineValue * control.ViewRange;
                this.HorizontalGap = this.ActualHeight;
                this.VerticalGap = this.ActualWidth / this.VerticalLines;

                base.OnRender(drawingContext);

                for (int i = 1; i <= control.ViewRange; i++)
                {
                    for (int j = 0; j < timeLineValue; j++)
                    {
                        var formattedTime = new FormattedText($" {TimeSpan.FromHours(j).ToString(@"hh\:mm")}", cultureInfo,
                            FlowDirection.LeftToRight, typeface, 10D, Brushes.Gray, pixelPerDip);

                        drawingContext.DrawText(formattedTime, renderPoint);
                        renderPoint.X += this.VerticalGap;
                    }
                }
            }
        }
    }
}
