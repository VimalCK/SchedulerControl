using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scheduler
{
    internal sealed class DateHeader : StackPanel
    {
        private double headerWidth;
        private int scheduleViewRange;
        private int currentHeaderIndex;
        private ScheduleControl templatedParent;
        private FrameworkElement currentHeader;
        private TranslateTransform transform;
        public DateHeader()
        {
            this.DefaultStyleKey = typeof(DateHeader);
            this.Orientation = Orientation.Horizontal;

            AddHandlers();
        }

        ~DateHeader()=>RemoveHandlers();

        internal void SetBorderColor(Brush brush)
        {
            if (brush is null)
            {
                return;
            }

            foreach (Label label in this.Children)
            {
                label.BorderBrush = brush;
            }
        }

        private void DateHeader_Loaded(object sender, RoutedEventArgs e)
        {
            this.CoerceTemplatedParent();
            this.CalculateScheduleViewDateRange();
            this.MeasureHeadersDesiredWidth();

            for (int i = 0; i < scheduleViewRange; i++)
            {
                var label = new Label()
                {
                    Content = $" {templatedParent.StartDate.AddDays(i).ToString("dd-MM-yyyy")}",
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 10,
                    Width = headerWidth,
                    BorderThickness = new Thickness(1),
                    Background = Brushes.White,
                    BorderBrush = Brushes.LightGray,
                    RenderTransform = new TranslateTransform(0, 0)
                };

                Panel.SetZIndex(label, i);
                this.Children.Add(label);
            }
        }


        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (currentHeader is null)
            {
                SetTransformElementValues();
            }

            var change = transform.X + e.HorizontalChange;

            switch (change)
            {
                case double c when c > headerWidth:
                    currentHeaderIndex++;
                    transform.X = headerWidth;
                    SetTransformElementValues();
                    transform.X = change - headerWidth;
                    break;
                case double c when c < 0:
                    currentHeaderIndex--;
                    transform.X = 0;
                    SetTransformElementValues();
                    transform.X += change;
                    break;
                default:
                    transform.X = change;
                    break;
            }
        }

        private void SetTransformElementValues()
        {
            currentHeader = (FrameworkElement)this.Children[currentHeaderIndex];
            transform = (TranslateTransform)currentHeader.RenderTransform;
        }

        private void AddHandlers(bool handleOnlyTemplatedParentEvents = false)
        {
            if (!handleOnlyTemplatedParentEvents)
            {
                this.Loaded += DateHeader_Loaded;
            }
            else
            {
                this.templatedParent.scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                this.templatedParent.SizeChanged += TemplatedParent_SizeChanged;
            }
        }

        private void RemoveHandlers()
        {
            this.Loaded -= DateHeader_Loaded;
            this.templatedParent.SizeChanged -= TemplatedParent_SizeChanged;
            this.templatedParent.scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
        }

        private void TemplatedParent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Children.Count == 0)
            {
                return;
            }

            headerWidth = e.NewSize.Width;

            foreach (FrameworkElement element in this.Children)
            {
                element.Width = headerWidth;
            }
        }

        private void CoerceTemplatedParent()
        {
            if (templatedParent == null)
            {
                templatedParent = this.TemplatedParent as ScheduleControl;
                AddHandlers(true);
            }
        }

        private void MeasureHeadersDesiredWidth() => headerWidth = this.ActualWidth / scheduleViewRange;

        private void CalculateScheduleViewDateRange() => this.scheduleViewRange = (templatedParent.EndDate - templatedParent.StartDate).Days + 1;

    }
}
