using System.Windows;
using System.Windows.Media;
using static Scheduler.UIExtensions;
using static Scheduler.Common.Values;

namespace Scheduler.Common
{
    public static class DrawingContextExtensions
    {
        public static void DrawBorder(this DrawingContext drawingContext, FrameworkElement visual, Brush brush, double thickness = .5)
        {
            var pen = new Pen(brush, thickness);
            drawingContext.DrawLine(pen, new Point(), new Point(visual.ActualWidth, Zero));
            drawingContext.DrawLine(pen, new Point(), new Point(Zero, visual.ActualHeight));
            drawingContext.DrawLine(pen, new Point(visual.ActualWidth, Zero), new Point(visual.ActualWidth, visual.ActualHeight));
            drawingContext.DrawLine(pen, new Point(Zero, visual.ActualHeight), new Point(visual.ActualWidth, visual.ActualHeight));
        }

        public static void DrawText(this DrawingContext drawingContext, Visual visual, string text, double x, double y)
        {
            drawingContext.DrawText(GetFormattedText(visual, text), new Point(x, y));
        }

        public static void PushClip(this DrawingContext drawingContext, double width, double height)
            => drawingContext.PushClip(new RectangleGeometry(new Rect(Zero, Zero, width, height)));
    }
}
