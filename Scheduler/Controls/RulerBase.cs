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
            Pen pen = new(RulerColor, .5);
            Point startPoint = new(0, 0);
            Point endPoint = new(0, ActualHeight);

            pen.Freeze();

            for (int i = 0; i <= VerticalLines; i++)
            {
                drawingContext.DrawLine(pen, startPoint, endPoint);
                endPoint.X = startPoint.X = i * VerticalGap;
            }

            startPoint = new(0, 0);
            endPoint = new(ActualWidth, 0);

            for (int i = 0; i < HorizontalLines; i++)
            {
                startPoint.Y = endPoint.Y = i * HorizontalGap;
                drawingContext.DrawLine(pen, startPoint, endPoint);
            }
        }

        protected internal abstract void Render();
    }
}
