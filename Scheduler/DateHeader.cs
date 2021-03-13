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
        private ScheduleControl parent;
        private FrameworkElement currentHeader;
        private TranslateTransform transform;
        public DateHeader()
        {
            DefaultStyleKey = typeof(DateHeader);
            Orientation = Orientation.Horizontal;

            AddHandlers();
        }

        ~DateHeader() => RemoveHandlers();

        internal void SetBorderColor(Brush brush)
        {
            if (brush is null)
            {
                return;
            }

            foreach (Label label in Children)
            {
                label.BorderBrush = brush;
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (currentHeader is null)
            {
                SetTransformElementValues();
            }

            var change = Math.Round(transform.X + e.HorizontalChange, 2);

            switch (change)
            {
                case double c when c > parent.ViewPortArea.Width:
                    if (currentHeaderIndex != Children.Count - 1) currentHeaderIndex++;
                    transform.X = parent.ViewPortArea.Width;
                    SetTransformElementValues();
                    transform.X = Math.Floor(change - parent.ViewPortArea.Width);
                    break;
                case double c when c < 0:
                    if (currentHeaderIndex != 0) currentHeaderIndex--;
                    transform.X = 0;
                    SetTransformElementValues();
                    transform.X = Math.Round(transform.X + change, 2);
                    break;
                default:
                    transform.X = change;
                    break;
            }
        }

        private void SetTransformElementValues()
        {
            currentHeader = (FrameworkElement)Children[currentHeaderIndex];
            transform = (TranslateTransform)currentHeader.RenderTransform;
        }

        private void AddHandlers(bool handleOnlyTemplatedParentEvents = false)
        {
            if (!handleOnlyTemplatedParentEvents)
            {
                SizeChanged += DateHeader_SizeChanged;
            }
            else
            {
                parent.scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }
        }

        private void DateHeader_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CoerceTemplatedParent();
            if (parent.ViewRange < 0)
            {
                return;
            }

            var existingCount = Children.Count;
            var requiredLabels = parent.ViewRange - existingCount;
            if (requiredLabels > 0)
            {
                requiredLabels += existingCount;
                for (int index = 0; index < requiredLabels; index++)
                {
                    Label label;
                    if (index < existingCount)
                    {
                        label = (Label)Children[index];
                    }
                    else
                    {
                        label = new Label()
                        {
                            BorderThickness = new Thickness(1),
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 10,
                            BorderBrush = parent.TimeLineColor,
                            RenderTransform = new TranslateTransform(0, 0)
                        };

                        Children.Add(label);
                    }

                    label.Content = $" {parent.StartDate.AddDays(index).ToString("dd-MM-yyyy")}";
                    label.Width = parent.ViewPortArea.Width;
                    Panel.SetZIndex(label, index);
                }
            }
            else if (requiredLabels < 0)
            {
                Children.RemoveRange(0, Math.Abs(requiredLabels));
                for (int index = 0; index < existingCount; index++)
                {
                    var label = (Label)Children[index];
                    label.Content = $" {parent.StartDate.AddDays(index).ToString("dd-MM-yyyy")}";
                    label.Width = parent.ViewPortArea.Width;
                }
            }
            else
            {
                foreach (FrameworkElement item in Children)
                {
                    item.Width = parent.ViewPortArea.Width;
                }
            }
        }

        private void RemoveHandlers()
        {
            SizeChanged -= DateHeader_SizeChanged;
            parent.scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
        }

        internal void ReArrangeHeader()
        {
            if (Children.Count == 0)
            {
                return;
            }

            foreach (FrameworkElement element in Children)
            {
                element.Width = ActualWidth;
            }
        }

        private void CoerceTemplatedParent()
        {
            if (parent == null)
            {
                parent = TemplatedParent as ScheduleControl;
                AddHandlers(true);
            }
        }
    }
}
