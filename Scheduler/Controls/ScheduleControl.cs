using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Scheduler
{
    [TemplatePart(Name = "PART_ContentSection", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_RulerGrid", Type = typeof(RulerGrid))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_DateHeader", Type = typeof(DateHeader))]
    [TemplatePart(Name = "PART_TimeRulerPanel", Type = typeof(TimeRulerPanel))]
    [TemplatePart(Name = "PART_TimeLineHeader", Type = typeof(TimeLineHeader))]
    [TemplatePart(Name = "PART_GroupHeader", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PART_HeaderSection", Type = typeof(Grid))]
    public class ScheduleControl : Control
    {
        public event ScrollChangedEventHandler ScrollChanged;

        internal ItemsControl groupHeader;
        //private Func<IAppointment, string> groupValueLambda;
        private SortedList<string, List<IAppointment>> appointments;
        private double scrollBarSpace;
        private Size viewPortArea;
        private Size requiredArea;
        private Grid contentSection;
        private RulerGrid rulerGrid;
        private DateHeader dateHeader;
        private TimeRulerPanel timerulerPanel;
        private TimeLineHeader timeLineHeader;
        private ScrollViewer scrollViewer;
        private Grid headerSection;



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

        public static readonly DependencyProperty GroupHeaderProperty = DependencyProperty.Register(
            "GroupHeader", typeof(string), typeof(ScheduleControl), new PropertyMetadata(OnGroupHeaderChanged));

        public static readonly DependencyProperty GroupResourcesProperty = DependencyProperty.Register(
            "GroupResources", typeof(IEnumerable), typeof(ScheduleControl), new PropertyMetadata(default(IEnumerable)));

        public static readonly DependencyProperty GroupHeaderContentTemplateProperty = DependencyProperty.Register(
            "GroupHeaderContentTemplate", typeof(DataTemplate), typeof(ScheduleControl), new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty ExtendedModeHeightProperty = DependencyProperty.Register(
            "ExtendedModeHeight", typeof(double), typeof(ScheduleControl), new PropertyMetadata((double)ExtendedMode.Normal));

        public IEnumerable GroupResources
        {
            get => (IEnumerable)GetValue(GroupResourcesProperty);
            set => SetValue(GroupResourcesProperty, value);
        }

        public double ExtendedModeHeight
        {
            get => (double)GetValue(ExtendedModeHeightProperty);
            private set => SetValue(ExtendedModeHeightProperty, value);
        }

        public DataTemplate GroupHeaderContentTemplate
        {
            get => (DataTemplate)GetValue(GroupHeaderContentTemplateProperty);
            set => SetValue(GroupHeaderContentTemplateProperty, value);
        }
        public string GroupHeader
        {
            get => (string)GetValue(GroupHeaderProperty);
            set => SetValue(GroupHeaderProperty, value);
        }

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
        public IEnumerable<string> Group => appointments?.Keys;

        internal int ViewRange => (EndDate.Date - StartDate.Date).Days + 1;

        public ScheduleControl()
        {
            DefaultStyleKey = typeof(ScheduleControl);
            TimeLineProviders = new FreezableCollection<TimeRuler>();
            appointments = new SortedList<string, List<IAppointment>>();
            //groupValueLambda = CommonFunctions.GetPropertyValue<IAppointment, string>(nameof(IAppointment.Group));
        }
        
        private static void OnIsExtendedModeChanges(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            control.ExtendedModeHeight = (double)(control.IsExtendedMode ? ExtendedMode.Zoom : ExtendedMode.Normal);
            control.rulerGrid?.InvalidateVisual();
        }

        private static void OnTimeLineZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            if (control.IsLoaded)
            {
                control.InvalidateChildControlsToReRender();
            }
        }
        private static void OnScheduleDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            if (control.IsLoaded)
            {
                control.InvalidateChildControlsToReRender();
                control.dateHeader.ReArrangeHeaders();
            }
        }

        private static void OnTimeLineProvidersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            control.timerulerPanel?.InvalidateVisual();
        }

        private static void OnTimeLineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            if (control.IsLoaded)
            {
                control.rulerGrid?.InvalidateVisual();
                control.timeLineHeader?.InvalidateVisual();
            }
        }

        private static void OnGroupHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private async static void OnAppointmentSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleControl control)
            {
                control.AppointmentSource.CollectionChanged += AppointmentSource_CollectionChanged;
                if (await control.AddGroupResources((IEnumerable<IAppointment>)e.NewValue))
                {
                    control.groupHeader.Items.Refresh();
                }

                control.InvalidateChildControlsToReRender();
            }
        }


        private async static void AppointmentSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            bool isAdded = false;
            bool isRemoved = false;
            var control = sender as ScheduleControl;
            if (!e.NewItems.Count.Equals(0))
            {
                isAdded = await control.AddGroupResources((IEnumerable<IAppointment>)e.NewItems);
            }

            if (!e.OldItems.Count.Equals(0))
            {
                foreach (IAppointment item in e.NewItems)
                {
                    isRemoved = await control.RemoveAppointmentsAndNotifyIfGroupChanged((IEnumerable<IAppointment>)e.OldItems);
                }
            }

            if (isRemoved || isAdded)
            {
                control.groupHeader.Items.Refresh();
            }
        }

        ~ScheduleControl() => UnHandleEvents();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            contentSection = GetTemplateChild("PART_ContentSection") as Grid;
            rulerGrid = GetTemplateChild("PART_RulerGrid") as RulerGrid;
            scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            dateHeader = GetTemplateChild("PART_DateHeader") as DateHeader;
            timerulerPanel = GetTemplateChild("PART_TimeRulerPanel") as TimeRulerPanel;
            timeLineHeader = GetTemplateChild("PART_TimeLineHeader") as TimeLineHeader;
            groupHeader = GetTemplateChild("PART_GroupHeader") as ItemsControl;
            headerSection = GetTemplateChild("PART_HeaderSection") as Grid;

            HandleEvents();
        }

        private async ValueTask<bool> AddGroupResources(IEnumerable source)
        {
            //if (source is null)
            //{
            //    return await Task<bool>.Run(() =>
            //     {
            //         bool isAdded = false;
            //         foreach (IAppointment appointment in source)
            //         {
            //             var group = groupValueLambda(appointment);
            //             if (appointments.ContainsKey(group))
            //             {
            //                 appointments[group].Add(appointment);
            //             }
            //             else
            //             {
            //                 appointments[group] = new List<IAppointment>(new[] { appointment });
            //                 isAdded = true;
            //             }
            //         }

            //         return isAdded;
            //     });
            //}
            return false;
        }

        private async ValueTask<bool> RemoveAppointmentsAndNotifyIfGroupChanged(IEnumerable<IAppointment> source)
        {
            //return await Task<bool>.Run(() =>
            //{
            //    bool isRemoved = false;
            //    foreach (IAppointment appointment in source)
            //    {
            //        var group = groupValueLambda(appointment);
            //        if (appointments.ContainsKey(group))
            //        {
            //            appointments[group].Remove(appointment);

            //        }

            //        if (!isRemoved && appointments[group].Count.Equals(0))
            //        {
            //            isRemoved = true;
            //        }
            //    }
            //});

            return false;
        }

        private void PrepareScheduleControl()
        {
            groupHeader.RenderTransform = new TranslateTransform();
            (GetTemplateChild("PART_ScrollGapMask") as Border).Width = scrollBarSpace;
        }

        private void HandleEvents()
        {
            Loaded += ScheduleControl_Loaded;
            SizeChanged += ScheduleControl_SizeChanged;
            scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (IsLoaded && e.Source is ScrollViewer)
            {
                ScrollChanged?.Invoke(sender, e);
                ScrollGroupHeaderVertically(-e.VerticalOffset);
            }


        }
        private void ScrollGroupHeaderVertically(double offset) => ((TranslateTransform)groupHeader.RenderTransform).Y = offset;

        private void ScheduleControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsLoaded)
            {
                InvalidateChildControlsToReRender();
            }
        }

        private void ScheduleControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ScheduleControl_Loaded;

            List<ScrollBar> scrollBars = new List<ScrollBar>();
            Helper.GetChildOfType<ScrollBar>(scrollViewer, ref scrollBars, level: 2);
            scrollBarSpace = scrollBars[0].Width;

            PrepareScheduleControl();
            InvalidateChildControlsToReRender();
            dateHeader.ReArrangeHeaders();
        }

        private void UnHandleEvents()
        {
            AppointmentSource.CollectionChanged -= AppointmentSource_CollectionChanged;
            scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
        }

        private void InvalidateChildControlsToReRender()
        {
            InvalidateViewPortArea();
            var width = requiredArea.Width * ViewRange;
            if (!contentSection.Width.Equals(width) || !contentSection.Height.Equals(requiredArea.Height))
            {
                contentSection.Width = width;
                headerSection.Width = width;
                contentSection.Height = requiredArea.Height;
            }
        }

        private void InvalidateViewPortArea()
        {
            viewPortArea.Width = scrollViewer.ActualWidth - scrollBarSpace;
            viewPortArea.Height = scrollViewer.ActualHeight - scrollBarSpace;

            var requiredHeight = groupHeader.Items.Count * this.ExtendedModeHeight;
            requiredArea.Height = requiredHeight < viewPortArea.Height ? viewPortArea.Height : requiredHeight;

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
