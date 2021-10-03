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
using Scheduler.Types;
using System.Threading;

namespace Scheduler
{
    [TemplatePart(Name = "PART_ContentSection", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_RulerGrid", Type = typeof(RulerGrid))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_DateHeader", Type = typeof(DateHeader))]
    [TemplatePart(Name = "PART_TimeRulerPanel", Type = typeof(TimeRulerPanel))]
    [TemplatePart(Name = "PART_TimeLineHeader", Type = typeof(TimeLineHeader))]
    [TemplatePart(Name = "PART_GroupHeader", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_HeaderSection", Type = typeof(Grid))]
    public class ScheduleControl : Control
    {
        public event ScrollChangedEventHandler ScrollChanged;

        private ListBox groupHeaderList;
        private Dictionary<Guid, GroupResource> appointmentStore;
        //private Func<IAppointment, string> groupValueLambda;
        private double scrollBarSpace;
        private Size viewPortArea;
        private Size requiredArea;
        private Grid contentSection;
        private RulerGrid rulerGrid;
        private DateHeader dateHeader;
        private TimeRulerPanel timerulerPanel;
        private TimeLineHeader timeLineHeader;
        private ScrollViewer schedulerScrollViewer;
        private ScrollViewer groupHeaderScrollViewer;
        private Grid headerSection;



        public static readonly DependencyProperty TimeLineColorProperty = DependencyProperty.Register(
            "TimeLineColor", typeof(Brush), typeof(ScheduleControl), new FrameworkPropertyMetadata(OnTimeLineColorChanged,
            (d, value) => value ?? Brushes.LightGray));

        public static readonly DependencyProperty StartDateProperty = DependencyProperty.Register(
            "StartDate", typeof(DateTime), typeof(ScheduleControl), new PropertyMetadata(DateTime.Now, OnScheduleDateChanged,
            (d, value) => value.Equals(default(DateTime)) ? DateTime.Now : value));

        public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register(
            "EndDate", typeof(DateTime), typeof(ScheduleControl), new PropertyMetadata(DateTime.Now, OnScheduleDateChanged,
            (d, value) => value.Equals(default(DateTime)) ? DateTime.Now : value));

        public static readonly DependencyProperty TimeLineZoomProperty = DependencyProperty.Register(
            "TimeLineZoom", typeof(TimeLineZoom), typeof(ScheduleControl), new PropertyMetadata(TimeLineZoom.TwentyFour, OnTimeLineZoomChanged,
            (d, value) => Enum.IsDefined(typeof(TimeLineZoom), value) ? value : TimeLineZoom.TwentyFour));

        public static readonly DependencyProperty ExtendedModeProperty = DependencyProperty.Register(
            "ExtendedMode", typeof(ExtendedMode), typeof(ScheduleControl), new PropertyMetadata(default(ExtendedMode), OnExtendedModeChanged,
            (d, value) => Enum.IsDefined(typeof(ExtendedMode), value) ? value : ExtendedMode.Normal));

        public static readonly DependencyProperty TimeLineProvidersProperty = DependencyProperty.Register(
            "TimeLineProviders", typeof(ObservableCollection<TimeRuler>), typeof(ScheduleControl),
            new PropertyMetadata(new ObservableCollection<TimeRuler>(), OnTimeLineProvidersChanged));

        public static readonly DependencyProperty AppointmentSourceProperty = DependencyProperty.Register(
            "AppointmentSource", typeof(ObservableCollection<Appointment>), typeof(ScheduleControl),
            new PropertyMetadata(new ObservableCollection<Appointment>(), OnAppointmentSourceChanged));

        public static readonly DependencyProperty GroupHeaderProperty = DependencyProperty.Register(
            "GroupHeader", typeof(string), typeof(ScheduleControl));

        public static readonly DependencyProperty GroupByProperty = DependencyProperty.Register(
            "GroupBy", typeof(ObservableCollection<GroupResource>), typeof(ScheduleControl),
            new PropertyMetadata(new ObservableCollection<GroupResource>(), OnGroupByChanged));

        public static readonly DependencyProperty GroupHeaderContentTemplateProperty = DependencyProperty.Register(
            "GroupHeaderContentTemplate", typeof(DataTemplate), typeof(ScheduleControl), new PropertyMetadata(default(DataTemplate)));

        public ExtendedMode ExtendedMode
        {
            get => (ExtendedMode)GetValue(ExtendedModeProperty);
            set => SetValue(ExtendedModeProperty, value);
        }

        public ObservableCollection<GroupResource> GroupBy
        {
            get => (ObservableCollection<GroupResource>)GetValue(GroupByProperty);
            set => SetValue(GroupByProperty, value);
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

        public ObservableCollection<Appointment> AppointmentSource
        {
            get => (ObservableCollection<Appointment>)GetValue(AppointmentSourceProperty);
            set => SetValue(AppointmentSourceProperty, value);
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

        public ObservableCollection<TimeRuler> TimeLineProviders
        {
            get => (ObservableCollection<TimeRuler>)GetValue(TimeLineProvidersProperty);
            set => SetValue(TimeLineProvidersProperty, value);
        }

        public Size ViewPortArea => viewPortArea;
        public Size RequiredArea => requiredArea;
        internal int ViewRange => (EndDate.Date - StartDate.Date).Days + 1;

        public ScheduleControl()
        {
            DefaultStyleKey = typeof(ScheduleControl);
            //groupValueLambda = CommonFunctions.GetPropertyValue<IAppointment, string>(nameof(IAppointment.Group));
        }

        private async static void OnGroupByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                CollectionChangedEventManager.RemoveHandler(oldCollection, control.GroupByCollectionChanged);
            }

            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                CollectionChangedEventManager.AddHandler(newCollection, control.GroupByCollectionChanged);
            }

            control.appointmentStore = new Dictionary<Guid, GroupResource>();
            await control.SyncGroupsInAppointmentStore((IEnumerable<GroupResource>)e.NewValue, null);
            await control.SyncAppointmentsInAppointmentsStore(control.AppointmentSource, null);
            //validate
            // control.InvalidateChildControlsToReRender();
        }

        private async void GroupByCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await SyncGroupsInAppointmentStore(e.NewItems?.OfType<GroupResource>().ToList(), e.OldItems?.OfType<GroupResource>().ToList());
            //validate
            //InvalidateChildControlsToReRender();
        }

        private async static void OnAppointmentSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleControl control)
            {
                if (e.OldValue is ObservableCollection<Appointment> oldAppointments)
                {
                    CollectionChangedEventManager.RemoveHandler(oldAppointments, control.AppointmentSource_CollectionChanged);
                }

                if (e.NewValue is ObservableCollection<Appointment> newAppointments)
                {
                    CollectionChangedEventManager.AddHandler(newAppointments, control.AppointmentSource_CollectionChanged);
                }

                await control.ClearAppointmentsFromAppointmentStore();
                await control.SyncAppointmentsInAppointmentsStore((IEnumerable<Appointment>)e.NewValue, null);
                //validate
                // control.InvalidateChildControlsToReRender();
            }
        }
        private async void AppointmentSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await SyncAppointmentsInAppointmentsStore((IEnumerable<Appointment>)e.NewItems, (IEnumerable<Appointment>)e.OldItems);
        }

        private async ValueTask SyncAppointmentsInAppointmentsStore(IEnumerable<Appointment> newItems, IEnumerable<Appointment> oldItems)
        {
            if (!oldItems.IsNullOrEmpty())
            {
                await Parallel.ForEachAsync(oldItems, new ParallelOptions { MaxDegreeOfParallelism = 2 },
                    (appointment, token) =>
                    {
                        if (appointmentStore.TryGetValue(appointment.Group.Id, out GroupResource group))
                        {
                            group.Appointments.Remove(appointment);
                        }

                        return ValueTask.CompletedTask;
                    });
            }

            if (!newItems.IsNullOrEmpty())
            {
                await Parallel.ForEachAsync(newItems, new ParallelOptions { MaxDegreeOfParallelism = 2 },
                    (appointment, token) =>
                    {
                        if (appointmentStore.TryGetValue(appointment.Group.Id, out GroupResource group))
                        {
                            group.Appointments.Add(appointment);
                        }

                        return ValueTask.CompletedTask;
                    });
            }
        }

        private async ValueTask ClearAppointmentsFromAppointmentStore()
        {
            await Parallel.ForEachAsync(appointmentStore, (group, token) =>
            {
                group.Value.Appointments.Clear();
                return ValueTask.CompletedTask;
            });
        }

        private async ValueTask SyncGroupsInAppointmentStore(IEnumerable<GroupResource> newItems, IEnumerable<GroupResource> oldItems)
        {
            if (!oldItems.IsNullOrEmpty())
            {
                await Parallel.ForEachAsync(oldItems, new ParallelOptions { MaxDegreeOfParallelism = 2 },
                    (group, token) =>
                    {
                        appointmentStore.Remove(group.Id);
                        return ValueTask.CompletedTask;
                    });
            }

            if (!newItems.IsNullOrEmpty())
            {
                await Parallel.ForEachAsync(newItems, new ParallelOptions { MaxDegreeOfParallelism = 2 },
                    (group, token) =>
                    {
                        appointmentStore.Add(group.Id, group);
                        return ValueTask.CompletedTask;
                    });
            }
        }

        private static void OnExtendedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            if (control.IsLoaded && !control.InvalidateChildControlsToReRender())
            {
                control.rulerGrid.Render();
            }
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
            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                CollectionChangedEventManager.RemoveHandler(oldCollection, control.TimeLineProvidersCollectionChanged);
            }

            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                CollectionChangedEventManager.AddHandler(newCollection, control.TimeLineProvidersCollectionChanged);
            }

            control.timerulerPanel?.Render();
        }

        private void TimeLineProvidersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => timerulerPanel?.Render();

        private static void OnTimeLineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            if (control.IsLoaded)
            {
                control.rulerGrid?.Render();
                control.timeLineHeader?.Render();
            }
        }

        ~ScheduleControl() => UnHandleEvents();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            contentSection = GetTemplateChild("PART_ContentSection") as Grid;
            rulerGrid = GetTemplateChild("PART_RulerGrid") as RulerGrid;
            schedulerScrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            dateHeader = GetTemplateChild("PART_DateHeader") as DateHeader;
            timerulerPanel = GetTemplateChild("PART_TimeRulerPanel") as TimeRulerPanel;
            timeLineHeader = GetTemplateChild("PART_TimeLineHeader") as TimeLineHeader;
            groupHeaderList = GetTemplateChild("PART_GroupHeader") as ListBox;
            headerSection = GetTemplateChild("PART_HeaderSection") as Grid;

            HandleEvents();
        }

        private void PrepareScheduleControl()
        {
            (GetTemplateChild("PART_HeaderSectionRightGapMask") as Border).Width = scrollBarSpace;
            (GetTemplateChild("PART_HeaderSectionBottomGapMask") as Border).Height = scrollBarSpace;
        }

        private void HandleEvents()
        {
            WeakEventManager<ScheduleControl, RoutedEventArgs>.AddHandler(this, nameof(Loaded), ScheduleControl_Loaded);
            WeakEventManager<ScheduleControl, SizeChangedEventArgs>.AddHandler(this, nameof(SizeChanged), ScheduleControl_SizeChanged);
            WeakEventManager<ScrollViewer, ScrollChangedEventArgs>.AddHandler(schedulerScrollViewer, nameof(ScrollChanged), ScrollViewer_ScrollChanged);
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (IsLoaded && e.Source is ScrollViewer)
            {
                ScrollChanged?.Invoke(this, e);
                ScrollGroupHeaderVertically(e.VerticalOffset);
            }
        }

        private void ScrollGroupHeaderVertically(double offset) => groupHeaderScrollViewer?.ScrollToVerticalOffset(offset);
        private void ScheduleControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsLoaded)
            {
                InvalidateChildControlsToReRender();
            }
        }

        private void ScheduleControl_Loaded(object sender, RoutedEventArgs e)
        {
            WeakEventManager<ScheduleControl, RoutedEventArgs>.RemoveHandler(this, nameof(Loaded), ScheduleControl_Loaded);

            var scrollbars = new List<ScrollBar>();
            groupHeaderScrollViewer = groupHeaderList.GetChildOfType<ScrollViewer>();
            schedulerScrollViewer.GetChildOfType<ScrollBar>(ref scrollbars, level: 2);
            scrollBarSpace = scrollbars.ElementAt(0).ActualWidth;

            PrepareScheduleControl();
            InvalidateChildControlsToReRender();
            dateHeader.ReArrangeHeaders();
        }

        private void UnHandleEvents()
        {
            CollectionChangedEventManager.RemoveHandler(AppointmentSource, AppointmentSource_CollectionChanged);
            WeakEventManager<ScrollViewer, ScrollChangedEventArgs>.RemoveHandler(schedulerScrollViewer, nameof(ScrollChanged), ScrollViewer_ScrollChanged);
            if (GroupBy is not null)
            {
                CollectionChangedEventManager.RemoveHandler(GroupBy, GroupByCollectionChanged);
            }

            if (TimeLineProviders is not null)
            {
                CollectionChangedEventManager.RemoveHandler(TimeLineProviders, TimeLineProvidersCollectionChanged);
            }
        }

        private bool InvalidateChildControlsToReRender()
        {
            CalculateViewPortSize();
            CalculateRequiredAreaSize();
            var width = requiredArea.Width * ViewRange;
            if (!contentSection.Width.Equals(width) || !contentSection.Height.Equals(requiredArea.Height))
            {
                contentSection.Width = width;
                headerSection.Width = width;
                contentSection.Height = requiredArea.Height;
                return true;
            }

            return false;
        }

        private void CalculateViewPortSize()
        {
            viewPortArea.Width = schedulerScrollViewer.ActualWidth - scrollBarSpace;
            viewPortArea.Height = schedulerScrollViewer.ActualHeight - scrollBarSpace;
        }

        private void CalculateRequiredAreaSize()
        {
            var requiredHeight = GroupBy.Count(g => g.Visibility == Visibility.Visible) * (int)this.ExtendedMode;
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
