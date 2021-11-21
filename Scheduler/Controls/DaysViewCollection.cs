using System;
using System.Windows;
using System.Windows.Controls;

namespace Scheduler
{
    internal sealed class DaysViewCollection : ListBox
    {
        private ScheduleControl parent;

        public DaysViewCollection()
        {
            this.DefaultStyleKey = typeof(ListBox);
        }

        internal void CreateDaysView()
        {
            int range = parent.ViewRange;
            int existingCount = Items.Count;
            int requiredItems = range - existingCount;
            if (requiredItems >= 0)
            {
                for (int index = 0; index < range; index++)
                {
                    if (index < existingCount)
                    {
                        Items[index] = $" {parent.StartDate.AddDays(index):dd-MM-yyyy}"; ;
                    }
                    else
                    {
                        Items.Add($" {parent.StartDate.AddDays(index):dd-MM-yyyy}");
                    }
                }
            }
            else if (requiredItems < 0)
            {
                requiredItems = Math.Abs(requiredItems);
                for (int index = 0; index < Items.Count; index++)
                {
                    if (index < requiredItems)
                    {
                        Items[index] = $" {parent.StartDate.AddDays(index):dd-MM-yyyy}";
                    }
                    else
                    {
                        Items.RemoveAt(index--);
                    }
                }
            }
        }

        public override void OnApplyTemplate()
        {
            var border = (Border)GetVisualChild(0);
            border.Padding = new(0);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            parent = (ScheduleControl)TemplatedParent;
        }

        protected override DependencyObject GetContainerForItemOverride() => new DayView();
        protected override bool IsItemItsOwnContainerOverride(object item) => item is DayView;
    }
}
