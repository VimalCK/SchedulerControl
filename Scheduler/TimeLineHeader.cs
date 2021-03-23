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

        public TimeLineHeader()
        {
            DefaultStyleKey = typeof(TimeLineHeader);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (TemplatedParent is ScheduleControl control && control.ActualWidth != 0)
            {
                VerticalLines = 24 * control.ViewRange;

                if (VerticalLines > 0)
                {
                    var clippingPoint = new Point();
                    var renderPoint = new Point(0, ActualHeight / 3);
                    var pixelPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
                    var headerText = 0;

                    HorizontalLines = 2;
                    HorizontalGap = ActualHeight;
                    RulerColor = control.TimeLineColor;
                    VerticalGap = control.ViewPortArea.Width / (int)control.TimeLineZoom;

                    base.OnRender(drawingContext);

                    for (int i = 0; i < VerticalLines; i++)
                    {
                        var formattedTime = new FormattedText($" {TimeSpan.FromHours(headerText).ToString(@"hh\:mm")}", cultureInfo,
                                FlowDirection.LeftToRight, typeface, 10D, Brushes.Gray, pixelPerDip);

                        drawingContext.PushClip(new RectangleGeometry(new Rect(clippingPoint, new Size
                        {
                            Width = VerticalGap - 3,
                            Height = ActualHeight
                        })));

                        drawingContext.DrawText(formattedTime, renderPoint);
                        drawingContext.Pop();
                        clippingPoint.X = renderPoint.X += VerticalGap;
                        headerText = headerText >= 23 ? 0 : headerText + 1;
                    }
                }
            }
        }
    }
}
