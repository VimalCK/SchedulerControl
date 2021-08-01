using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Linq;

namespace Scheduler
{
    internal sealed class DateHeader : Grid
    {
        private ScheduleControl templatedParent;

        public DateHeader()
        {
            DefaultStyleKey = typeof(DateHeader);
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
                        ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                        label = new Label()
                        {
                            BorderThickness = new Thickness(.5, .5, .5, 0),
                            VerticalAlignment = VerticalAlignment.Stretch,
                            VerticalContentAlignment = VerticalAlignment.Center,
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

                    label.Content = $" {templatedParent.StartDate.AddDays(index):dd-MM-yyyy}";
                    label.Background = index % 2 == 0 ? Brushes.Green : Brushes.Red;
                }
            }
            else if (requiredItems < 0)
            {
                requiredItems = Math.Abs(requiredItems);
                Children.RemoveRange(templatedParent.ViewRange, requiredItems);
                ColumnDefinitions.RemoveRange(templatedParent.ViewRange, requiredItems);
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
            var previousHeaderIndex = (int)((e.HorizontalOffset - e.HorizontalChange) / templatedParent.RequiredArea.Width);
            var currentHeaderIndex = (int)(e.HorizontalOffset / templatedParent.RequiredArea.Width);
            var combinedHeaderWidth = templatedParent.RequiredArea.Width * (currentHeaderIndex + 1);
            var offsetChange = (e.HorizontalOffset + templatedParent.RequiredArea.Width) - combinedHeaderWidth;
            TranslateTransform headerTransform;

            if (RestrictTransition(e, offsetChange))
            {
                headerTransform = (TranslateTransform)Children[currentHeaderIndex].RenderTransform;
                if (!headerTransform.X.Equals(0))
                {
                    headerTransform.X = 0;
                }

                return;
            }

            headerTransform = (TranslateTransform)Children[previousHeaderIndex].RenderTransform;
            switch (headerTransform.X + e.HorizontalChange)
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
                    headerTransform.X = offsetChange;
                    break;
            }
        }

        private bool RestrictTransition(ScrollChangedEventArgs e, double offsetChange) =>
            e.HorizontalOffset.Equals(0) || offsetChange.Equals(0) || offsetChange > templatedParent.RequiredArea.Width;


        private void RemoveHandlers()
        {
            templatedParent.ScrollChanged -= ScrollViewer_ScrollChanged;
        }
    }
}
