using System;
using System.Windows;
using System.Windows.Controls;

namespace Scheduler.Behaviors
{
    public sealed class PanelOptions
    {
        public static bool GetDisableScrolling(DependencyObject obj)
            => (bool)obj.GetValue(DisableScrollingProperty);

        public static void SetDisableScrolling(DependencyObject obj, bool value)
            => obj.SetValue(DisableScrollingProperty, value);

        public static bool GetEnableVirtualization(DependencyObject obj)
            => (bool)obj.GetValue(EnableVirtualizationProperty);

        public static void SetEnableVirtualization(DependencyObject obj, bool value)
            => obj.SetValue(EnableVirtualizationProperty, value);

        public static readonly DependencyProperty EnableVirtualizationProperty =
            DependencyProperty.RegisterAttached("EnableVirtualization", typeof(bool), typeof(PanelOptions),
                new PropertyMetadata(OnEnableVirtualizationChanged));

        public static readonly DependencyProperty DisableScrollingProperty =
            DependencyProperty.RegisterAttached("DisableScrolling", typeof(bool), typeof(PanelOptions),
               new PropertyMetadata(OnDisableScrollingChanged));


        private static void OnEnableVirtualizationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if ((bool)e.NewValue == true)
                {
                    VirtualizingPanel.SetIsVirtualizing(element, true);
                    VirtualizingPanel.SetVirtualizationMode(element, VirtualizationMode.Recycling);
                    VirtualizingPanel.SetScrollUnit(element, ScrollUnit.Pixel);
                    return;
                }

                VirtualizingPanel.SetIsVirtualizing(element, false);
            }
        }

        private static void OnDisableScrollingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if ((bool)e.NewValue == true)
                {
                    ScrollViewer.SetHorizontalScrollBarVisibility(element, ScrollBarVisibility.Hidden);
                    ScrollViewer.SetVerticalScrollBarVisibility(element, ScrollBarVisibility.Hidden);
                    HandleScrollingEvents(element, true);
                }
                else
                {
                    ScrollViewer.SetHorizontalScrollBarVisibility(element, ScrollBarVisibility.Auto);
                    ScrollViewer.SetVerticalScrollBarVisibility(element, ScrollBarVisibility.Auto);
                    HandleScrollingEvents(element, false);
                }
            }
        }

        private static void Element_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                element.Unloaded -= Element_Unloaded;
                element.PreviewMouseWheel -= Element_PreviewMouseWheel;
                element.PreviewKeyDown -= Element_PreviewKeyDown;
            }
        }

        private static void HandleScrollingEvents(FrameworkElement element, bool value)
        {
            if (value)
            {
                element.Unloaded += Element_Unloaded;
                element.PreviewMouseWheel += Element_PreviewMouseWheel;
                element.PreviewKeyDown += Element_PreviewKeyDown;
                return;
            }

            element.Unloaded -= Element_Unloaded;
            element.PreviewMouseWheel -= Element_PreviewMouseWheel;
            element.PreviewKeyDown -= Element_PreviewKeyDown;
        }

        private static void Element_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
            => e.Handled = true;

        private static void Element_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
           => e.Handled = true;
    }
}
