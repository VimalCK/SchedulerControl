using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
            var range = parent.ViewRange;
            if (range <= 0)
            {
                return;
            }

            var existingCount = Children.Count;
            var requiredItems = range - existingCount;
            if (requiredItems > 0)
            {
                for (int index = 0; index < range; index++)
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
                            BorderThickness = new Thickness(.5, .5, .5, 0),
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 10,
                            Background = Brushes.White,
                            RenderTransform = new TranslateTransform(0, 0)
                        };

                        Children.Add(label);
                        Grid.SetColumn(label, index);
                        Panel.SetZIndex(label, index);
                        label.SetBinding(Label.BorderBrushProperty, new Binding
                        {
                            Path = new PropertyPath("TimeLineColor"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ScheduleControl), 1)
                        });
                    }

                    label.Content = $" {parent.StartDate.AddDays(index).ToString("dd-MM-yyyy")}";
                }
            }
            else if (requiredItems < 0)
            {
                requiredItems = Math.Abs(requiredItems);
                Children.RemoveRange(parent.ViewRange, requiredItems);
                ColumnDefinitions.RemoveRange(parent.ViewRange, requiredItems);
                for (int index = 0; index < Children.Count; index++)
                {
                    var label = (Label)Children[index];
                    label.Content = $" {parent.StartDate.AddDays(index).ToString("dd-MM-yyyy")}";
                }
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
                    case double c when c > parent.RequiredArea.Width:
                        transform.X = parent.RequiredArea.Width;
                        TrySettingTransformElementValues(++currentHeaderIndex);
                        transform.X = change - parent.RequiredArea.Width;
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
