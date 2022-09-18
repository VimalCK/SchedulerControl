using System;
using System.Windows.Media;
using static Scheduler.Common.Values;

namespace Scheduler
{
    internal sealed class RulerGrid : RulerBase
    {
        private ScheduleControl parent;
        private readonly DrawingGroup backingStore = new();

        public RulerGrid() => DefaultStyleKey = typeof(RulerGrid);

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            parent = TemplatedParent as ScheduleControl;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (parent is null || ActualWidth.Equals(Zero))
            {
                return;
            }

            Render();
            drawingContext.DrawDrawing(backingStore);
        }

        protected internal override void Render()
        {
            VerticalLines = 24 * parent.ViewRange;
            if (VerticalLines > 0)
            {
                var drawingContext = backingStore.Open();
                HorizontalGap = (int)parent.ExtendedMode;
                HorizontalLines = (int)Math.Round(parent.RequiredViewPortArea.Height / HorizontalGap);
                VerticalGap = parent.ViewPortArea.Width / (int)parent.TimeLineZoom;
                RulerColor = parent.TimeLineColor;
                base.OnRender(drawingContext);
                drawingContext.Close();
            }
        }
    }
}
