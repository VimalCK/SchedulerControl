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
            Background = Brushes.Red;
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
                        ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        label = new ContentTransformLabel();
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

        private void DateHeader_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= DateHeader_Loaded;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var currentIndex = (short)(e.HorizontalOffset / templatedParent.RequiredArea.Width);
            var change = (templatedParent.RequiredArea.Width + e.HorizontalOffset) - (templatedParent.RequiredArea.Width * (currentIndex + 1));
            var previousIndex = (short)((e.HorizontalOffset - e.HorizontalChange) / templatedParent.RequiredArea.Width);
            if (change.Equals(0))
            {
                ((ContentTransformLabel)Children[previousIndex]).HorizontalContentOffset = 0;
                return;
            }

            var label = (ContentTransformLabel)Children[currentIndex];
            label.HorizontalContentOffset = change;
            if (!previousIndex.Equals(currentIndex))
            {
                ((ContentTransformLabel)Children[previousIndex]).HorizontalContentOffset = 0;
            }

            //var change = !e.ExtentWidthChange.Equals(0) ? -e.ExtentWidthChange : e.HorizontalChange;
            //var currentHeaderIndex = (int)(e.HorizontalOffset / templatedParent.RequiredArea.Width);
            //var offsetChange = (templatedParent.RequiredArea.Width + e.HorizontalOffset) - (templatedParent.RequiredArea.Width * (currentHeaderIndex + 1));
            //var previousHeaderIndex = new RangeInt(0, (short)(Children.Count - 1), (short)((e.HorizontalOffset - change) / templatedParent.RequiredArea.Width));
            //var label = (ContentTransformLabel)Children[previousHeaderIndex];
            //if (RestrictTransition(e, offsetChange))
            //{
            //    label.HorizontalContentOffset = 0;
            //    return;
            //}

            //switch (label.HorizontalContentOffset + change)
            //{
            //    case double c when c > templatedParent.RequiredArea.Width:
            //        label.HorizontalContentOffset = 0;
            //        label = (ContentTransformLabel)Children[currentHeaderIndex];
            //        label.HorizontalContentOffset = offsetChange;
            //        break;
            //    case double c when c < 0:
            //        label.HorizontalContentOffset = 0;
            //        label = (ContentTransformLabel)Children[currentHeaderIndex];
            //        label.HorizontalContentOffset = offsetChange;
            //        break;
            //    default:
            //        label.HorizontalContentOffset = offsetChange;
            //        break;
            //}
        }

        private bool RestrictTransition(ScrollChangedEventArgs e, double offsetChange) =>
            e.HorizontalOffset.Equals(0) || offsetChange.Equals(0) || offsetChange > templatedParent.RequiredArea.Width;


        private void RemoveHandlers()
        {
            templatedParent.ScrollChanged -= ScrollViewer_ScrollChanged;
        }
    }
}
