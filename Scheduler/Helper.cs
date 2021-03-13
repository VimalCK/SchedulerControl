using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Scheduler
{
    public static class Helper
    {
        public static bool GetChildOfType<T>(this DependencyObject control,ref List<T> items) where T : DependencyObject
        {
            if (control == null)
            {
                return false;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(control); i++)
            {
                var child = VisualTreeHelper.GetChild(control, i);
                var result = child as T;
                if (result is null)
                {
                    GetChildOfType<T>(child, ref items);
                }
                else
                {
                    items.Add(result);
                }
            }

            return items.Count > 0;
        }
    }
}
