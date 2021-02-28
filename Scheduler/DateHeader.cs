using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Scheduler
{
    internal sealed class DateHeader : FrameworkElement
    {
        public DateHeader()
        {
            this.DefaultStyleKey = typeof(DateHeader);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (this.TemplatedParent is ScheduleControl control && control.ActualWidth != 0)
            {
                var pen = new Pen(control.TimeLineColor ?? Brushes.LightGray, .5);
                var startPoint = new Point(0, 0);
                var endPoint = new Point(0, this.ActualHeight);
                var noOfDays = (control.EndDate.Day - control.StartDate.Day) + 1;
                var pixelPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
                var rulerGap = this.ActualWidth / 2;
                pen.Freeze();

                for (int interval = 0; interval <= noOfDays; interval++)
                {
                    endPoint.X = startPoint.X = interval * rulerGap;

                    var formattedTime = new FormattedText($"{DateTime.Now.AddDays(interval).ToString()}", new CultureInfo("en-US"), FlowDirection.LeftToRight, new Typeface("Arial"), 10D, Brushes.Gray, pixelPerDip);

                    drawingContext.DrawText(formattedTime, startPoint);
                    drawingContext.DrawLine(pen, startPoint, endPoint);
                }

                //var horizontalRulerHeight = this.RulerMinimumHeight <= 0 ? this.ActualHeight : this.RulerMinimumHeight;
                //var noOfRows = this.ActualHeight / horizontalRulerHeight;
                //startPoint = new Point(0, 0);
                //endPoint = new Point(control.ActualWidth * noOfDays, 0);
                //for (rulerGap = 0; rulerGap <= noOfRows; rulerGap++)
                //{
                //    startPoint.Y = endPoint.Y = rulerGap * horizontalRulerHeight;
                //    drawingContext.DrawLine(pen, startPoint, endPoint);
                //}
            }
        }
    }
}
