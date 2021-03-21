﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Scheduler
{
    [TemplatePart(Name = "PART_ParentGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_RulerGrid", Type = typeof(RulerGrid))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_DateHeader", Type = typeof(DateHeader))]
    [TemplatePart(Name = "PART_TimeRulerPanel", Type = typeof(TimeRulerPanel))]
    [TemplatePart(Name = "PART_TimeLineHeader", Type = typeof(TimeLineHeader))]
    [TemplatePart(Name = "PART_Canvas", Type = typeof(AppointmentRenderingCanvas))]
    public class ScheduleControl : Control
    {
        private ScrollBar horizontalScrollBar;
        private ScrollBar verticalScrollBar;
        private Size viewPortArea;
        private Size requiredArea;
        private Grid parentGrid;
        private RulerGrid rulerGrid;
        private DateHeader dateHeader;
        private TimeRulerPanel timerulerPanel;
        private TimeLineHeader timeLineHeader;
        private AppointmentRenderingCanvas canvas;

        internal ScrollViewer scrollViewer;


        public static readonly DependencyProperty TimeLineColorProperty = DependencyProperty.Register(
            "TimeLineColor", typeof(Brush), typeof(ScheduleControl), new FrameworkPropertyMetadata(Brushes.LightGray, OnTimeLineColorChanged));

        public static readonly DependencyProperty StartDateProperty = DependencyProperty.Register(
              "StartDate", typeof(DateTime), typeof(ScheduleControl), new PropertyMetadata(DateTime.Now, OnScheduleDateChanged));

        public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register(
               "EndDate", typeof(DateTime), typeof(ScheduleControl), new PropertyMetadata(DateTime.Now, OnScheduleDateChanged));

        public static readonly DependencyProperty TimeLineZoomProperty = DependencyProperty.Register(
            "TimeLineZoom", typeof(TimeLineZoom), typeof(ScheduleControl), new PropertyMetadata(TimeLineZoom.TwentyFour, OnTimeLineZoomChanged));

        public static readonly DependencyProperty IsExtendedModeProperty = DependencyProperty.Register(
            "IsExtendedMode", typeof(bool), typeof(ScheduleControl), new PropertyMetadata(default(bool), OnIsExtendedModeChanges));

        public static readonly DependencyProperty TimeLineProvidersProperty = DependencyProperty.Register(
            "TimeLineProviders", typeof(FreezableCollection<TimeRuler>), typeof(ScheduleControl), new PropertyMetadata(OnTimeLineProvidersChanged));

        public static readonly DependencyProperty AppointmentSourceProperty = DependencyProperty.Register(
            "AppointmentSource", typeof(ObservableCollection<IAppointment>), typeof(ScheduleControl),
            new PropertyMetadata(new ObservableCollection<IAppointment>(), OnAppointmentSourceChanged));

        public ObservableCollection<IAppointment> AppointmentSource
        {
            get => (ObservableCollection<IAppointment>)GetValue(AppointmentSourceProperty);
            set => SetValue(AppointmentSourceProperty, value);
        }

        public bool IsExtendedMode
        {
            get => (bool)GetValue(IsExtendedModeProperty);
            set => SetValue(IsExtendedModeProperty, value);
        }

        public TimeLineZoom TimeLineZoom
        {
            get => (TimeLineZoom)GetValue(TimeLineZoomProperty);
            set => SetValue(TimeLineZoomProperty, value);
        }

        public DateTime EndDate
        {
            get => (DateTime)GetValue(EndDateProperty);
            set => SetValue(EndDateProperty, value);
        }

        public DateTime StartDate
        {
            get => (DateTime)GetValue(StartDateProperty);
            set => SetValue(StartDateProperty, value);
        }

        public Brush TimeLineColor
        {
            get => (Brush)GetValue(TimeLineColorProperty);
            set => SetValue(TimeLineColorProperty, value);
        }

        public FreezableCollection<TimeRuler> TimeLineProviders
        {
            get => (FreezableCollection<TimeRuler>)GetValue(TimeLineProvidersProperty);
            set => SetValue(TimeLineProvidersProperty, value);
        }

        public Size ViewPortArea => viewPortArea;

        public Size RequiredArea => requiredArea;

        internal int ViewRange => (EndDate.Date - StartDate.Date).Days + 1;

        public ScheduleControl()
        {
            DefaultStyleKey = typeof(ScheduleControl);
            TimeLineProviders = new FreezableCollection<TimeRuler>();
        }

        private static void OnIsExtendedModeChanges(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            control.rulerGrid?.InvalidateVisual();
        }

        private static void OnTimeLineZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            control.InvalidateChildControlsToReRender();
        }
        private static void OnScheduleDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            control.InvalidateChildControlsToReRender();
        }

        private static void OnTimeLineProvidersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            control.timerulerPanel?.InvalidateVisual();
        }

        private static void OnTimeLineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            control.rulerGrid?.InvalidateVisual();
            control.timeLineHeader?.InvalidateVisual();
        }

        private static void OnAppointmentSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleControl control)
            {
            }
        }

        private static void AppointmentSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        ~ScheduleControl() => UnHandleEvents();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            parentGrid = GetTemplateChild("PART_ParentGrid") as Grid;
            rulerGrid = GetTemplateChild("PART_RulerGrid") as RulerGrid;
            scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            dateHeader = GetTemplateChild("PART_DateHeader") as DateHeader;
            timerulerPanel = GetTemplateChild("PART_TimeRulerPanel") as TimeRulerPanel;
            timeLineHeader = GetTemplateChild("PART_TimeLineHeader") as TimeLineHeader;
            canvas = GetTemplateChild("PART_Canvas") as AppointmentRenderingCanvas;

            HandleEvents();
        }

        private void HandleEvents()
        {
            SizeChanged += ScheduleControl_SizeChanged;
            Loaded += ScheduleControl_Loaded;
            AppointmentSource.CollectionChanged += AppointmentSource_CollectionChanged;
        }

        private void UnHandleEvents()
        {
            SizeChanged -= ScheduleControl_SizeChanged;
            Loaded -= ScheduleControl_Loaded;
            AppointmentSource.CollectionChanged -= AppointmentSource_CollectionChanged;
        }

        private void ScheduleControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (horizontalScrollBar == null || verticalScrollBar == null)
            {
                List<ScrollBar> scrollBars = new List<ScrollBar>();
                Helper.GetChildOfType<ScrollBar>(scrollViewer, ref scrollBars);
                foreach (var scrollBar in scrollBars)
                {
                    if (scrollBar.Orientation == Orientation.Horizontal)
                    {
                        horizontalScrollBar = scrollBar;
                    }
                    else
                    {
                        verticalScrollBar = scrollBar;
                    }
                }

            }

            InvalidateChildControlsToReRender();
        }

        private void ScheduleControl_Loaded(object sender, RoutedEventArgs e)
        {
            InvalidateChildControlsToReRender();
        }

        private void InvalidateChildControlsToReRender()
        {
            InvalidateViewPortArea();
            if (parentGrid.Width == RequiredArea.Width * ViewRange)
            {
                timeLineHeader.InvalidateVisual();
                rulerGrid.InvalidateVisual();
            }
            else
            {
                parentGrid.Width = RequiredArea.Width * ViewRange;
            }

            dateHeader.ReArrangeHeaders();
        }

        private void InvalidateViewPortArea()
        {
            viewPortArea.Width = scrollViewer.ActualWidth - verticalScrollBar.ActualWidth;
            viewPortArea.Height = scrollViewer.ActualHeight - horizontalScrollBar.ActualHeight;

            switch (TimeLineZoom)
            {
                case TimeLineZoom.Twelve:
                    requiredArea.Width = viewPortArea.Width * 2;
                    break;
                case TimeLineZoom.TwentyFour:
                    requiredArea.Width = viewPortArea.Width;
                    break;
                case TimeLineZoom.FortyEight:
                    if (ViewRange.Equals(1))
                    {
                        TimeLineZoom = TimeLineZoom.TwentyFour;
                        break;
                    }

                    requiredArea.Width = viewPortArea.Width / 2;
                    break;
                default:
                    break;
            }
        }
    }
}
