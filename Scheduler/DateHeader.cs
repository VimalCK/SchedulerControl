using System;
using System.ComponentModel;
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
            Loaded += DateHeader_Loaded;
            var dp = DependencyPropertyDescriptor.FromProperty(DateHeader.ActualWidthProperty, typeof(DateHeader));
            dp.AddValueChanged(this, Test);
        }

        private void Test(object sender, EventArgs e)
        {
            if (parent.ViewRange > 0)
            {
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
                                BorderBrush = Brushes.Black,
                                Background = index % 2 == 0 ? Brushes.LightGreen : Brushes.LightYellow,
                                RenderTransform = new TranslateTransform(0, 0)
                            };

                            Children.Add(label);
                        }

                        label.Content = $" {parent.StartDate.AddDays(index).ToString("dd-MM-yyyy")}";
                        //label.Width = parent.ViewPortArea.Width;
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
                        //item.Width = parent.ViewPortArea.Width;
                    }
                }
            }
        }

        ~DateHeader() => RemoveHandlers();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            parent = (ScheduleControl)TemplatedParent;
        }

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

        private void DateHeaderActualWidthChanged(object sender, EventArgs e)
        {

        }

        private void DateHeader_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= DateHeader_Loaded;
            parent.scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (TrySettingTransformElementValues())
            {
                var change = transform.X + e.HorizontalChange;

                switch (change)
                {
                    case double c when c > parent.ViewPortArea.Width:
                        transform.X = parent.ViewPortArea.Width;
                        TrySettingTransformElementValues(++currentHeaderIndex);
                        transform.X = change - parent.ViewPortArea.Width;
                        break;
                    case double c when c < 0:
                        transform.X = 0;
                        TrySettingTransformElementValues(--currentHeaderIndex);
                        transform.X += change;
                        break;
                    default:
                        transform.X = change;
                        break;
                }
            }
        }

        private bool TrySettingTransformElementValues(int elementIndex = 0)
        {
            if (Children.Count == 0 || elementIndex < 0 || elementIndex > Children.Count)
            {
                return false;
            }

            currentHeader = (FrameworkElement)Children[currentHeaderIndex];
            transform = (TranslateTransform)currentHeader.RenderTransform;
            return true;
        }

        private void RemoveHandlers()
        {
            parent.scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
            DependencyPropertyDescriptor.FromProperty(DateHeader.ActualWidthProperty,
                typeof(DateHeader)).RemoveValueChanged(this, DateHeaderActualWidthChanged);
        }
    }
}
