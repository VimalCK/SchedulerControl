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
        private RangeInt headerRange;
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

            headerRange = new RangeInt(0, (short)(Children.Count - 1), headerRange.CurrentIndex);
        }

        private void DateHeader_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= DateHeader_Loaded;

        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var change = !e.HorizontalChange.Equals(0) ? e.HorizontalChange : -(e.ViewportWidthChange * headerRange);
            TranslateTransform transform = (TranslateTransform)Children[headerRange].RenderTransform;
            change = transform.X + change;

            if (RestrictTransform(change, e))
            {
                transform.X = 0;
                return;
            }

            switch (change)
            {
                case double c when c >= Math.Round(templatedParent.RequiredArea.Width, 10):
                    transform.X = 0;
                    transform = (TranslateTransform)Children[++headerRange].RenderTransform;
                    transform.X = change - templatedParent.RequiredArea.Width;

                    break;
                case double c when c <= 0:
                    transform.X = 0;
                    transform = (TranslateTransform)Children[--headerRange].RenderTransform;
                    transform.X = change + templatedParent.RequiredArea.Width;

                    break;
                default:
                    transform.X = change;
                    break;
            }
        }

        private bool RestrictTransform(double change, ScrollChangedEventArgs e) =>
            (headerRange.Equals(0) && change <= 0) ||
            (headerRange.Equals(headerRange.Upper) && e.ViewportWidthChange < 0);

        private void RemoveHandlers()
        {
            templatedParent.ScrollChanged -= ScrollViewer_ScrollChanged;
        }
    }
}
