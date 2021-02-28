using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Scheduler
{
    internal sealed class DateHeader : RulerBase
    {
        public DateHeader()
        {
            this.DefaultStyleKey = typeof(DateHeader);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.TemplatedParent is ScheduleControl control && control.ActualWidth != 0)
            {
                var pixelPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
                var point = new Point(5, this.ActualHeight / 3);

                this.VerticalLines = (control.EndDate - control.StartDate).Days + 1;
                this.VerticalGap = this.ActualWidth / this.VerticalLines;
                this.HorizontalGap = this.ActualHeight;

                for (int i = 0; i < this.VerticalLines; i++)
                {
                    var formattedTime = new FormattedText($"{DateTime.Now.AddDays(i).Date.ToString("d")}", new CultureInfo("en-US"),
                        FlowDirection.LeftToRight, new Typeface("Arial"), 10D, Brushes.Gray, pixelPerDip);

                    point.X += this.VerticalGap * i;
                    drawingContext.DrawText(formattedTime, point);
                }

                base.OnRender(drawingContext);
            }
        }
    }
}
