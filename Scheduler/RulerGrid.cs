using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Scheduler
{
    internal sealed class RulerGrid : RulerBase
    {
        public RulerGrid()
        {
            this.DefaultStyleKey = typeof(RulerGrid);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.TemplatedParent is ScheduleControl control && control.ActualWidth > 0)
            {
                var noOfDays = (control.EndDate - control.StartDate).Days + 1;
                var timeLineValue = (int)control.TimeLineZoom;

                this.VerticalLines = timeLineValue * noOfDays;
                this.HorizontalLines = 10;
                this.HorizontalGap = this.ActualHeight/ this.HorizontalLines;
                this.VerticalGap = this.ActualWidth / this.VerticalLines;

                base.OnRender(drawingContext);
            }
        }
    }
}
