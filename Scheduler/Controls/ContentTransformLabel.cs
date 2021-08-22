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
            drawingGroup = new DrawingGroup();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            CreateVisual();
            drawingContext.DrawDrawing(drawingGroup);
        }

        private void CreateVisual()
        {
            var drawingContext = drawingGroup.Open();
            var point1 = new Point();
            var point2 = new Point();
            var pen = new Pen(BorderBrush, BorderThickness.Left);

            if (BorderThickness.Equals(new Thickness(BorderThickness.Left)))
            {
                drawingContext.DrawRectangle(Background, pen, new Rect(this.RenderSize));
            }
            else
            {
                drawingContext.DrawRectangle(Background, null, new Rect(this.RenderSize));

                // Left border
                point2.Y = ActualHeight;
                drawingContext.DrawLine(pen, point1, point2);

                // Top border
                point2.X = ActualWidth;
                point2.Y = 0;
                drawingContext.DrawLine(pen, point1, point2);

                // Right border
                point1.X = ActualWidth;
                point2.X = ActualWidth;
                point2.Y = ActualHeight;
                drawingContext.DrawLine(pen, point1, point2);

                // Bottom border
                point1.X = 0;
                point1.Y = ActualHeight;
                drawingContext.DrawLine(pen, point1, point2);
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
