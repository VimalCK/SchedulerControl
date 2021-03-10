using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scheduler
{
    internal sealed class DateHeader : StackPanel
    {
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

        ~DateHeader() => RemoveHandlers();

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

            for (int i = 1; i <= this.templatedParent.ViewRange; i++)
            {
                var label = new Label()
                {
                    Content = $" {templatedParent.StartDate.AddDays(i).ToString("dd-MM-yyyy")}",
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 10,
                    Width = this.ActualWidth,
                    BorderBrush = this.templatedParent.TimeLineColor,
                    BorderThickness = new Thickness(1, 1, i == this.templatedParent.ViewRange ? 1 : 0, 1),
                    Background = i % 2 == 0 ? Brushes.LightBlue : Brushes.LightGoldenrodYellow,
                    RenderTransform = new TranslateTransform(0, 0)
                };

                Panel.SetZIndex(label, i);
                this.Children.Add(label);
            }
        }


        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //if (currentHeader is null)
            //{
            //    SetTransformElementValues();
            //}

            //var change = Math.Round(transform.X + e.HorizontalChange, 2);

            //switch (change)
            //{
            //    case double c when c > headerWidth:
            //        if (currentHeaderIndex != this.Children.Count - 1) currentHeaderIndex++;
            //        transform.X = headerWidth;
            //        SetTransformElementValues();
            //        transform.X = Math.Floor(change - headerWidth);
            //        break;
            //    case double c when c < 0:
            //        if (currentHeaderIndex != 0) currentHeaderIndex--;
            //        transform.X = 0;
            //        SetTransformElementValues();
            //        transform.X = Math.Round(transform.X + change, 2);
            //        break;
            //    default:
            //        transform.X = change;
            //        break;
            //}
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
            }
        }

        private void RemoveHandlers()
        {
            this.Loaded -= DateHeader_Loaded;
            this.templatedParent.scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
        }

        internal void ReArrangeHeader()
        {
            if (this.Children.Count == 0)
            {
                return;
            }

            foreach (FrameworkElement element in this.Children)
            {
                element.Width = this.ActualWidth;
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
    }
}
