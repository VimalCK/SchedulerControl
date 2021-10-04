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
        private TranslateTransform transform;

        public DateHeader()
        {
            DefaultStyleKey = typeof(DateHeader);
            transform = new();
            RenderTransform = transform;
            Loaded += DateHeaderLoaded;
        }

        ~DateHeader() => RemoveHandlers();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            templatedParent = (ScheduleControl)TemplatedParent;
            templatedParent.ScrollChanged += ScrollViewerScrollChanged;
        }

        internal void ReArrangeHeaders()
        {
            int range = templatedParent.ViewRange;
            int existingCount = Children.Count;
            int requiredItems = range - existingCount;
            if (requiredItems >= 0)
            {
                ContentTransformLabel label;
                for (int index = 0; index < range; index++)
                {
                    if (index < existingCount)
                    {
                        label = (ContentTransformLabel)Children[index];
                    }
                    else
                    {
                        ColumnDefinitions.Add(new() { Width = new GridLength(1, GridUnitType.Star) });
                        label = new();
                        Children.Add(label);
                        Grid.SetColumn(label, index);
                    }

                    label.Content = $" {templatedParent.StartDate.AddDays(index):dd-MM-yyyy}";
                }
            }
            else if (requiredItems < 0)
            {
                requiredItems = Math.Abs(requiredItems);
                ColumnDefinitions.RemoveRange(templatedParent.ViewRange, requiredItems);
                Children.RemoveRange(templatedParent.ViewRange, requiredItems);
                for (int index = 0; index < Children.Count; index++)
                {
                    var label = (ContentTransformLabel)Children[index];
                    label.Content = $" {templatedParent.StartDate.AddDays(index):dd-MM-yyyy}";
                }
            }
        }

        private void DateHeaderLoaded(object sender, RoutedEventArgs e) => Loaded -= DateHeaderLoaded;

        private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var currentIndex = (short)(e.HorizontalOffset / templatedParent.RequiredArea.Width);
            var change = (templatedParent.RequiredArea.Width + e.HorizontalOffset) - (templatedParent.RequiredArea.Width * (currentIndex + 1));
            var previousIndex = (short)((e.HorizontalOffset - e.HorizontalChange) / templatedParent.RequiredArea.Width);
            transform.X = -e.HorizontalOffset;
            if (change.Equals(0))
            {
                DefaultHorizontalOffset();
                return;
            }

            var label = (ContentTransformLabel)Children[currentIndex];
            label.HorizontalContentOffset = change;
            if (!previousIndex.Equals(currentIndex))
            {
                DefaultHorizontalOffset();
            }

            void DefaultHorizontalOffset()
            {
                if (previousIndex > Children.Count - 1)
                {
                    previousIndex = --currentIndex;
                }

               ((ContentTransformLabel)Children[previousIndex]).HorizontalContentOffset = 0;
            }
        }

        private bool RestrictTransition(ScrollChangedEventArgs e, double offsetChange) =>
            e.HorizontalOffset.Equals(0) || offsetChange.Equals(0) || offsetChange > templatedParent.RequiredArea.Width;


        private void RemoveHandlers() => templatedParent.ScrollChanged -= ScrollViewerScrollChanged;
    }
}
