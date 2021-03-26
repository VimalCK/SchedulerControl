using System;
using System.Windows.Media;

namespace Scheduler
{
    internal sealed class RulerGrid : RulerBase
    {
        public RulerGrid() => DefaultStyleKey = typeof(RulerGrid);

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (TemplatedParent is ScheduleControl control && control.ActualWidth > 0)
            {
                VerticalLines = 24 * control.ViewRange;
                if (VerticalLines > 0)
                {
                    HorizontalGap = control.ExtendedModeSize;
                    HorizontalLines = (int)Math.Round(ActualHeight / HorizontalGap);
                    VerticalGap = control.ViewPortArea.Width / (int)control.TimeLineZoom;
                    RulerColor = control.TimeLineColor;
                    base.OnRender(drawingContext);
                }
            }
        }
    }
}
