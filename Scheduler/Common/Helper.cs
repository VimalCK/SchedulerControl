using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Scheduler
{
    public static class Helper
    {
        private static double pixelsPerDpi;
        private static Typeface typeface;
        private static CultureInfo cultureInfo;

        public static CultureInfo CultureInfo
        {
            get
            {
                if (cultureInfo == null)
                {
                    cultureInfo = new CultureInfo("en-US");
                }

                return cultureInfo;
            }
        }

        public static Typeface Typeface
        {
            get
            {
                if (typeface == null)
                {
                    typeface = new Typeface("Arial");
                }

                return typeface;
            }
        }

        public static bool GetChildOfType<T>(this DependencyObject control, ref List<T> items, int level = int.MaxValue) where T : DependencyObject
        {
            if (control == null || level.Equals(-1))
            {
                return false;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(control); i++)
            {
                var child = VisualTreeHelper.GetChild(control, i);
                var result = child as T;
                if (result is null)
                {
                    GetChildOfType<T>(child, ref items, --level);
                    level++;
                }
                else
                {
                    items.Add(result);
                }
            }

            return items.Count > 0;
        }

        public static T GetChildOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            if (element == null)
            {
                return default;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                if (child is not T)
                {
                    child = GetChildOfType<T>(child);
                }

                if (child is T)
                {
                    return (T)child;
                }
            }

            return default;
        }

        public static T GetParentOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            if (element is null)
            {
                return default;
            }

            if (element is T)
            {
                return (T)element;
            }

            var foundParent = GetParentOfType<T>((element as FrameworkElement)?.TemplatedParent);
            return foundParent;
        }

        public static double GetPixelsPerDpi(Visual visual)
        {
            if (pixelsPerDpi.Equals(0))
            {
                pixelsPerDpi = VisualTreeHelper.GetDpi(visual).PixelsPerDip;
            }

            return pixelsPerDpi;
        }

        public static bool IsNullOrEmpty(this IEnumerable value) => value is null || !value.GetEnumerator().MoveNext();
    }
}
