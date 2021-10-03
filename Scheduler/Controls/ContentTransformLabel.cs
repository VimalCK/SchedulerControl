using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Scheduler
{
    internal sealed class ContentTransformLabel : FrameworkElement
    {
        private DrawingGroup drawingGroup;
        private double horizontalContentOffset;
        private TranslateTransform transform;


        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(ContentTransformLabel), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(ContentTransformLabel), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            "BorderThickness", typeof(Thickness), typeof(ContentTransformLabel), new PropertyMetadata(default(Thickness)));

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(string), typeof(ContentTransformLabel), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsRender));


        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        public Thickness BorderThickness
        {
            get => (Thickness)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public string Content
        {
            get => (string)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public double HorizontalContentOffset
        {
            get => horizontalContentOffset;
            set
            {
                horizontalContentOffset = value;
                if (transform?.X != horizontalContentOffset)
                {
                    transform.X = horizontalContentOffset;
                }
            }
        }

        public ContentTransformLabel()
        {
            DefaultStyleKey = typeof(ContentTransformLabel);
            drawingGroup = new();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            CreateVisual();
            drawingContext.DrawDrawing(drawingGroup);
        }

        private void CreateVisual()
        {
            var drawingContext = drawingGroup.Open();
            Pen pen = new(BorderBrush, BorderThickness.Left);
            if (BorderThickness.Equals(new Thickness(BorderThickness.Left)))
            {
                drawingContext.DrawRectangle(Background, pen, new(this.RenderSize));
            }
            else
            {
                Point point1 = new();
                Point point2 = new();
                drawingContext.DrawRectangle(Background, null, new(this.RenderSize));

                if (BorderThickness.Left != 0)
                {
                    pen = new(BorderBrush, BorderThickness.Left);
                    point2.Y = ActualHeight;
                    point1.X = point1.Y = point2.X = 0;
                    drawingContext.DrawLine(pen, point1, point2);
                }

                if (BorderThickness.Top != 0)
                {
                    pen = new(BorderBrush, BorderThickness.Top);
                    point2.X = ActualWidth;
                    point1.X = point1.Y = point2.Y = 0;
                    drawingContext.DrawLine(pen, point1, point2);
                }

                if (BorderThickness.Right != 0)
                {
                    pen = new(BorderBrush, BorderThickness.Right);
                    point1.Y = 0;
                    point2.Y = ActualHeight;
                    point1.X = point2.X = ActualWidth;
                    drawingContext.DrawLine(pen, point1, point2);
                }

                if (BorderThickness.Bottom != 0)
                {
                    pen = new(BorderBrush, BorderThickness.Bottom);
                    point1.X = 0;
                    point2.X = ActualWidth;
                    point1.Y = point2.Y = ActualHeight;
                    drawingContext.DrawLine(pen, point1, point2);
                }
            }

            drawingContext.PushTransform(new TranslateTransform());
            drawingContext.DrawText(new FormattedText(Content ?? string.Empty, Helper.CultureInfo, FlowDirection.LeftToRight,
                 Helper.Typeface, 10, Brushes.Gray, Helper.GetPixelsPerDpi(this)), new Point(0, ActualHeight / 3));

            drawingContext.Pop();
            drawingContext.Close();

            transform = (drawingGroup.Children.Last() as DrawingGroup)?.Transform as TranslateTransform;
        }
    }
}
