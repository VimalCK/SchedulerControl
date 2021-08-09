using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Linq;

namespace Scheduler
{
    internal sealed class DateHeader : StackPanel
    {
        private ScheduleControl templatedParent;

        public DateHeader()
        {
            DefaultStyleKey = typeof(DateHeader);
            Orientation = Orientation.Horizontal;
            Loaded += DateHeader_Loaded;
        }

        ~DateHeader() => RemoveHandlers();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            templatedParent = (ScheduleControl)TemplatedParent;
            templatedParent.ScrollChanged += ScrollViewer_ScrollChanged;
        }

        internal void ReArrangeHeaders()
        {
            int range = templatedParent.ViewRange;
            if (range.Equals(0))
            {
                return;
            }

            int existingCount = Children.Count;
            int requiredItems = range - existingCount;
            if (requiredItems >= 0)
            {
                Label label;
                for (int index = 0; index < range; index++)
                {
                    if (index < existingCount)
                    {
                        label = (Label)Children[index];
                    }
                    else
                    {
                        label = new Label()
                        {
                            BorderThickness = new Thickness(.5),
                            VerticalAlignment = VerticalAlignment.Stretch,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            FontSize = 10,
                            Background = Brushes.White,
                            RenderTransform = new TranslateTransform(0, 0)
                        };

                        Children.Add(label);
                        Panel.SetZIndex(label, index);
                        label.SetBinding(Label.BorderBrushProperty, new Binding
                        {
                            Path = new PropertyPath("TimeLineColor"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ScheduleControl), 1)
                        });
                    }

                    label.Content = $" {templatedParent.StartDate.AddDays(index):dd-MM-yyyy}";
                    label.Background = index % 2 == 0 ? Brushes.Green : Brushes.Red;
                    label.Width = templatedParent.RequiredArea.Width;
                }
            }
            else if (requiredItems < 0)
            {
                requiredItems = Math.Abs(requiredItems);
                Children.RemoveRange(templatedParent.ViewRange, requiredItems);
                for (int index = 0; index < Children.Count; index++)
                {
                    var label = (Label)Children[index];
                    label.Content = $" {templatedParent.StartDate.AddDays(index):dd-MM-yyyy}";
                }
            }
        }

        private void DateHeader_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= DateHeader_Loaded;

        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var change = e.HorizontalChange;
            if (!e.ExtentWidthChange.Equals(0))
            {
                UpdateDateHeaderWidth();
                change = -e.ExtentWidthChange;
            }

            var currentHeaderIndex = (int)(e.HorizontalOffset / templatedParent.RequiredArea.Width);
            var offsetChange = (templatedParent.RequiredArea.Width + e.HorizontalOffset) - (templatedParent.RequiredArea.Width * (currentHeaderIndex + 1));
            var previousHeaderIndex = new RangeInt(0, (short)(Children.Count - 1), (short)((e.HorizontalOffset - change) / templatedParent.RequiredArea.Width));
            var headerTransform = (TranslateTransform)Children[previousHeaderIndex].RenderTransform;
            if (RestrictTransition(e, offsetChange))
            {
                headerTransform.X = 0;
                return;
            }

            switch (headerTransform.X + change)
            {
                case double c when c > templatedParent.RequiredArea.Width:
                    headerTransform.X = 0;
                    headerTransform = (TranslateTransform)Children[currentHeaderIndex].RenderTransform;
                    headerTransform.X = offsetChange;
                    break;
                case double c when c < 0:
                    headerTransform.X = 0;
                    headerTransform = (TranslateTransform)Children[currentHeaderIndex].RenderTransform;
                    headerTransform.X = offsetChange;
                    break;
                default:
                    headerTransform.X = offsetChange - .01;
                    break;
            }
        }

        private bool RestrictTransition(ScrollChangedEventArgs e, double offsetChange) =>
            e.HorizontalOffset.Equals(0) || offsetChange.Equals(0) || offsetChange > templatedParent.RequiredArea.Width;


        private void RemoveHandlers()
        {
            templatedParent.ScrollChanged -= ScrollViewer_ScrollChanged;
        }

        private void UpdateDateHeaderWidth()
        {
            foreach (Label item in this.Children)
            {
                item.Width = templatedParent.RequiredArea.Width;
            }
        }
    }
}
