using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Expression = System.Linq.Expressions.Expression;

namespace Scheduler
{
    public static class CommonExtensions
    {
        public static Func<TType, TProperty> GetPropertyValue<TType, TProperty>(string propertyName)
        {
            var sourceObjectParam = Expression.Parameter(typeof(TType));
            var prop = Expression.Property(sourceObjectParam, propertyName);
            return Expression.Lambda<Func<TType, TProperty>>(prop, sourceObjectParam).Compile();
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

                if (child is T t)
                {
                    return t;
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

            if (element is T t)
            {
                return t;
            }

            var foundParent = GetParentOfType<T>((element as FrameworkElement)?.TemplatedParent);
            return foundParent;
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

        public static bool IsNullOrEmpty(this IEnumerable value)
           => value is null || !value.GetEnumerator().MoveNext();
    }
}
