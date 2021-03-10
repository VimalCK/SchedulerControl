﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scheduler
{
    [TemplatePart(Name = "PART_ParentGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_RulerGrid", Type = typeof(RulerGrid))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_DateHeader", Type = typeof(DateHeader))]
    [TemplatePart(Name = "PART_TimeRulerPanel", Type = typeof(TimeRulerPanel))]
    [TemplatePart(Name = "PART_TimeLineHeader", Type = typeof(TimeLineHeader))]
    public class ScheduleControl : Control
    {
        private Grid parentGrid;
        private RulerGrid rulerGrid;
        private DateHeader dateHeader;
        private TimeRulerPanel timerulerPanel;
        private TimeLineHeader timeLineHeader;

        internal ScrollViewer scrollViewer;


        public static readonly DependencyProperty TimeLineColorProperty = DependencyProperty.Register(
            "TimeLineColor", typeof(Brush), typeof(ScheduleControl),
            new FrameworkPropertyMetadata(Brushes.LightGray, FrameworkPropertyMetadataOptions.AffectsRender, OnTimeLineColorChanged));

        public static readonly DependencyProperty StartDateProperty = DependencyProperty.Register(
              "StartDate", typeof(DateTime), typeof(ScheduleControl), new PropertyMetadata(OnScheduleDateChanged));

        public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register(
               "EndDate", typeof(DateTime), typeof(ScheduleControl), new PropertyMetadata(OnScheduleDateChanged));

        public static readonly DependencyProperty TimeLineZoomProperty = DependencyProperty.Register(
            "TimeLineZoom", typeof(TimeLineZoom), typeof(ScheduleControl), new PropertyMetadata(TimeLineZoom.FortyEight, OnTimeLineZoomChanged));

        public static readonly DependencyProperty TimeLineProvidersProperty =
             DependencyProperty.Register("ShadowTimeLineProviders", typeof(FreezableCollection<TimeRuler>), typeof(ScheduleControl),
                 new PropertyMetadata(OnTimeLineProvidersChanged));

        private static void OnTimeLineZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            control.rulerGrid?.InvalidateVisual();
            control.timeLineHeader?.InvalidateVisual();
        }
        private static void OnScheduleDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            control.rulerGrid?.InvalidateVisual();
            control.timeLineHeader?.InvalidateVisual();
            control.dateHeader?.ReArrangeHeader();
        }

        private static void OnTimeLineProvidersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleControl control && control.timerulerPanel != null)
            {
                control.InitiateTimeRulerRendering(e.NewValue as IEnumerable<TimeRuler>);
            }
        }

        public static FreezableCollection<TimeRuler> GetTimeLineProviders(DependencyObject d)
        {
            var items = (FreezableCollection<TimeRuler>)d.GetValue(TimeLineProvidersProperty);
            if (items is null)
            {
                items = new FreezableCollection<TimeRuler>();
                d.SetValue(TimeLineProvidersProperty, items);
            }

            return items;
        }
        public static void SetTimeLineProviders(DependencyObject d, FreezableCollection<TimeRuler> value) =>
            d.SetValue(TimeLineProvidersProperty, value);

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

        internal int ViewRange => (this.EndDate - this.StartDate).Days + 1;

        public ScheduleControl() => this.DefaultStyleKey = typeof(ScheduleControl);

        private static void OnTimeLineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            control.rulerGrid?.InvalidateVisual();
            control.timeLineHeader?.InvalidateVisual();
            control.dateHeader?.SetBorderColor(control.TimeLineColor);
        }

        ~ScheduleControl() => UnHandleEvents();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.parentGrid = this.GetTemplateChild("PART_ParentGrid") as Grid;
            this.rulerGrid = this.GetTemplateChild("PART_RulerGrid") as RulerGrid;
            this.scrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            this.dateHeader = this.GetTemplateChild("PART_DateHeader") as DateHeader;
            this.timerulerPanel = this.GetTemplateChild("PART_TimeRulerPanel") as TimeRulerPanel;
            this.timeLineHeader = this.GetTemplateChild("PART_TimeLineHeader") as TimeLineHeader;

            HandleEvents();
        }

        private void HandleEvents()
        {
            this.SizeChanged += ScheduleControl_SizeChanged;
            this.Loaded += ScheduleControl_Loaded;
        }

        private void UnHandleEvents()
        {
            this.SizeChanged -= ScheduleControl_SizeChanged;
            this.Loaded -= ScheduleControl_Loaded;
        }

        private void ScheduleControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.rulerGrid?.InvalidateVisual();
            this.timeLineHeader?.InvalidateVisual();
            this.dateHeader?.ReArrangeHeader();
        }

        private void ScheduleControl_Loaded(object sender, RoutedEventArgs e) =>
            InitiateTimeRulerRendering(GetTimeLineProviders(sender as DependencyObject));

        private void InitiateTimeRulerRendering(IEnumerable<TimeRuler> timeRulers) =>
            this.timerulerPanel.TimeRulers = new ObservableCollection<TimeRuler>(timeRulers);
    }
}
