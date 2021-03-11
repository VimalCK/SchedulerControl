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

                this.VerticalLines = 24 * control.ViewRange;
                this.HorizontalLines = 2;
                this.HorizontalGap = this.ActualHeight;
                this.VerticalGap = (this.ActualWidth / control.ViewRange) / timeLineValue;

                if (this.VerticalLines > 0)
                {
                    int headerText = 0;

                    base.OnRender(drawingContext);

                    for (int i = 0; i < this.VerticalLines; i++)
                    {
                        var formattedTime = new FormattedText($" {TimeSpan.FromHours(headerText).ToString(@"hh\:mm")}", cultureInfo,
                                FlowDirection.LeftToRight, typeface, 10D, Brushes.Gray, pixelPerDip);

                        drawingContext.DrawText(formattedTime, renderPoint);
                        renderPoint.X += this.VerticalGap;
                        headerText = headerText >= 23 ? 0 : headerText + 1;
                    }
                }
            }
        }
    }
}
