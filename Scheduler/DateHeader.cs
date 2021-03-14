using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scheduler
{
    internal sealed class DateHeader : Grid
    {
        private int currentHeaderIndex;
        private ScheduleControl parent;
        private TranslateTransform transform;

        public DateHeader()
        {
            DefaultStyleKey = typeof(DateHeader);
            Loaded += DateHeader_Loaded;
        }

        ~DateHeader() => RemoveHandlers();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            parent = (ScheduleControl)TemplatedParent;
        }

        internal void ReArrangeHeaders()
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
                            ColumnDefinitions.Add(new ColumnDefinition
                            {
                                Width = new GridLength(1, GridUnitType.Star)
                            });

                            label = new Label()
                            {
                                BorderThickness = new Thickness(1),
                                VerticalAlignment = VerticalAlignment.Center,
                                FontSize = 10,
                                Background = Brushes.White,
                                BorderBrush = parent.TimeLineColor,
                                RenderTransform = new TranslateTransform(0, 0)
                            };

                            Children.Add(label);
                        }

                        label.Content = $" {parent.StartDate.AddDays(index).ToString("dd-MM-yyyy")}";
                        Grid.SetColumn(label, index);
                        Panel.SetZIndex(label, index);
                    }
                }
                else if (requiredLabels < 0)
                {
                    Children.RemoveRange(parent.ViewRange, Math.Abs(requiredLabels));
                    ColumnDefinitions.RemoveRange(parent.ViewRange, Math.Abs(requiredLabels));
                    for (int index = 0; index < existingCount; index++)
                    {
                        var label = (Label)Children[index];
                        label.Content = $" {parent.StartDate.AddDays(index).ToString("dd-MM-yyyy")}";
                    }
                }
            }
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

            transform = (TranslateTransform)Children[currentHeaderIndex].RenderTransform;
            return true;
        }

        private void RemoveHandlers()
        {
            parent.scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
        }
    }
}
