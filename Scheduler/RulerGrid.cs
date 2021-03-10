using System.Windows.Media;

namespace Scheduler
{
    internal sealed class RulerGrid : RulerBase
    {
        public RulerGrid() => this.DefaultStyleKey = typeof(RulerGrid);

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.TemplatedParent is ScheduleControl control && control.ActualWidth > 0)
            {
                var timeLineValue = (int)control.TimeLineZoom;

                this.VerticalLines = timeLineValue * control.ViewRange;
                this.HorizontalLines = (int)this.ActualHeight / 30;
                this.HorizontalGap = this.ActualHeight / this.HorizontalLines;
                this.VerticalGap = this.ActualWidth / this.VerticalLines;

                base.OnRender(drawingContext);
            }
        }
    }
}
