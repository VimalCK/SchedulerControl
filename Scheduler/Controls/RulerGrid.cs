using System;
using System.Windows.Media;

namespace Scheduler
{
    internal sealed class RulerGrid : RulerBase
    {
        private DrawingGroup backingStore = new();

        public RulerGrid() => DefaultStyleKey = typeof(RulerGrid);

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (TemplatedParent is ScheduleControl control && control.ActualWidth > 0)
            {
                Render();
                drawingContext.DrawDrawing(backingStore);
            }
        }

        protected internal override void Render()
        {
            if (TemplatedParent is ScheduleControl control && control.ActualWidth > 0)
            {
                VerticalLines = 25 * control.ViewRange;
                if (VerticalLines > 0)
                {
                    var drawingContext = backingStore.Open();
                    HorizontalGap = (int)control.ExtendedMode;
                    HorizontalLines = (int)Math.Round(control.RequiredViewPortArea.Height / HorizontalGap);
                    VerticalGap = control.ViewPortArea.Width / (int)control.TimeLineZoom;
                    RulerColor = control.TimeLineColor;
                    base.OnRender(drawingContext);
                    drawingContext.Close();
                }
            }

        }
    }
}
