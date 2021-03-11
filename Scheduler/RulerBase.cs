using System;
using System.Windows;
using System.Windows.Media;

namespace Scheduler
{
    internal abstract class RulerBase : FrameworkElement
    {
        public Brush RulerColor
        {
            get => (Brush)GetValue(RulerColorProperty);
            set => SetValue(RulerColorProperty, value);
        }

        public int VerticalLines
        {
            get => (int)GetValue(VerticalLinesProperty);
            set => SetValue(VerticalLinesProperty, value);
        }

        public int HorizontalLines
        {
            get => (int)GetValue(HorizontalLinesProperty);
            set => SetValue(HorizontalLinesProperty, value);
        }

        public double VerticalGap
        {
            get => (double)GetValue(VerticalGapProperty);
            set => SetValue(VerticalGapProperty, value);
        }

        public double HorizontalGap
        {
            get => (double)GetValue(HorizontalGapProperty);
            set => SetValue(HorizontalGapProperty, value);
        }


        public static readonly DependencyProperty HorizontalGapProperty =
            DependencyProperty.Register("HorizontalGap", typeof(double), typeof(RulerBase),
                new FrameworkPropertyMetadata(default(double)));

        public static readonly DependencyProperty VerticalGapProperty =
            DependencyProperty.Register("VerticalGap", typeof(double), typeof(RulerBase),
                new FrameworkPropertyMetadata(default(double)));

        public static readonly DependencyProperty HorizontalLinesProperty =
            DependencyProperty.Register("HorizontalLines", typeof(int), typeof(RulerBase),
                new FrameworkPropertyMetadata(default(int)));

        public static readonly DependencyProperty VerticalLinesProperty =
            DependencyProperty.Register("VerticalLines", typeof(int), typeof(RulerBase),
                new FrameworkPropertyMetadata(default(int)));

        public static readonly DependencyProperty RulerColorProperty =
            DependencyProperty.Register("RulerColor", typeof(Brush), typeof(RulerBase),
                new FrameworkPropertyMetadata(Brushes.LightGray));

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var pen = new Pen(this.RulerColor, .5);
            var startPoint = new Point(0, 0);
            var endPoint = new Point(0, this.ActualHeight);
            var pixelPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            pen.Freeze();

            for (int i = 0; i <= this.VerticalLines; i++)
            {
                endPoint.X = startPoint.X = i * this.VerticalGap;
                drawingContext.DrawLine(pen, startPoint, endPoint);
            }

            startPoint = new Point(0, 0);
            endPoint = new Point(this.ActualWidth, 0);

            for (int i = 0; i < this.HorizontalLines; i++)
            {
                startPoint.Y = endPoint.Y = i * this.HorizontalGap;
                drawingContext.DrawLine(pen, startPoint, endPoint);
            }
        }
    }
}
