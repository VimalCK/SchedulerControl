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
                    HorizontalGap = control.IsExtendedMode ? 60 : 30;
                    HorizontalLines = (int)Math.Round(ActualHeight / HorizontalGap);
                    VerticalGap = (ActualWidth / control.ViewRange) / (int)control.TimeLineZoom;
                    RulerColor = control.TimeLineColor;
                    base.OnRender(drawingContext);
                }
            }
        }
    }
}
