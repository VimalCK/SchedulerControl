using Scheduler.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Scheduler
{
    [TemplatePart(Name = "PART_ContentSection", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_RulerGrid", Type = typeof(RulerGrid))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_DateHeader", Type = typeof(DateHeader))]
    [TemplatePart(Name = "PART_TimeRulerPanel", Type = typeof(TimeRulerPanel))]
    [TemplatePart(Name = "PART_TimeLineHeader", Type = typeof(TimeLineHeader))]
    [TemplatePart(Name = "PART_GroupContainer", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_HeaderSection", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_AppointmentRenderingCanvas", Type = typeof(AppointmentRenderingCanvas))]
    [TemplatePart(Name = "PART_AppointmentContainer", Type = typeof(ListBox))]
    public class ScheduleControl : Control
    {
        public event ScrollChangedEventHandler ScrollChanged;

        private Size requiredViewPortArea;
        private Size viewPortArea;
        private double scrollBarSpace;
        private OrderedDictionary<Guid, GroupResource> appointmentStore;
        private ListBox groupByContainer;
        //private Func<IAppointment, string> groupValueLambda;
        private Grid contentSection;
        private RulerGrid rulerGrid;
        private DateHeader dateHeader;
        private TimeRulerPanel timerulerPanel;
        private TimeLineHeader timeLineHeader;
        private ScrollViewer schedulerScrollViewer;
        private ScrollViewer groupContainerScrollViewer;
        private Grid headerSection;
        private AppointmentRenderingCanvas appointmentRenderingCanvas;
        private ListBox appointmentContainer;



        public static readonly DependencyProperty TimeLineColorProperty = DependencyProperty.Register(
            "TimeLineColor", typeof(Brush), typeof(ScheduleControl), new FrameworkPropertyMetadata(OnTimeLineColorChanged,
            (d, value) => value ?? Brushes.LightGray));

        public static readonly DependencyProperty StartDateProperty = DependencyProperty.Register(
            "StartDate", typeof(DateTime), typeof(ScheduleControl), new PropertyMetadata(DateTime.Now, OnScheduleDateChanged, CoerceScheduleDates));

        public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register(
            "EndDate", typeof(DateTime), typeof(ScheduleControl), new PropertyMetadata(DateTime.Now, OnScheduleDateChanged, CoerceScheduleDates));

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
        public Size RequiredViewPortArea => requiredViewPortArea;
        internal int ViewRange => (EndDate.Date - StartDate.Date).Days + 1;

        public ScheduleControl()
        {
            DefaultStyleKey = typeof(ScheduleControl);
            appointmentStore = new();
            //groupValueLambda = CommonFunctions.GetPropertyValue<IAppointment, string>(nameof(IAppointment.Group));
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
            groupByContainer = GetTemplateChild("PART_GroupContainer") as ListBox;
            headerSection = GetTemplateChild("PART_HeaderSection") as Grid;
            appointmentContainer = GetTemplateChild("PART_AppointmentContainer") as ListBox;

            HandleEvents();
        }

        private static void OnGroupByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            if (control.IsLoaded)
            {
                control.InvalidateChildControlsArea();
                control.appointmentStore = new();
                if (e.OldValue is INotifyCollectionChanged oldCollection)
                {
                    oldCollection.CollectionChanged -= control.GroupByCollectionChanged;
                }

                if (e.NewValue is INotifyCollectionChanged newCollection)
                {
                    newCollection.CollectionChanged += control.GroupByCollectionChanged;
                    control.SyncGroupsInAppointmentStore(e.NewValue as IEnumerable);
                    control.SyncAndRenderAppointments();
                }
                else
                {
                    control.AppointmentSource.AsParallel().ForAll(appointment => appointment.Hide());
                }
            }
        }

        private void GroupByCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                InvalidateChildControlsArea();
                if (e.Action.Equals(NotifyCollectionChangedAction.Add))
                {
                    var groupResource = (GroupResource)e.NewItems[0];
                    appointmentStore.Insert(e.NewStartingIndex, groupResource.Id, groupResource);
                    SyncAppointmentsInAppointmentsStore(AppointmentSource.Where(a => a.Group.Id == groupResource.Id));
                    appointmentRenderingCanvas.Render(AppointmentSource.Where(a => a.Group.Order >= e.NewStartingIndex));
                }
                else if (e.Action.Equals(NotifyCollectionChangedAction.Remove))
                {
                    var groupResource = (GroupResource)e.OldItems[0];
                    groupResource.Appointments.AsParallel().ForAll(appointment => appointment.Hide());
                    appointmentStore.Remove(groupResource.Id);
                }
            }
        }

        private static void OnAppointmentSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            if (control.IsLoaded)
            {
                if (e.OldValue is INotifyCollectionChanged oldAppointments)
                {
                    oldAppointments.CollectionChanged -= control.AppointmentSourceCollectionChanged;
                    control.ClearAppointmentsFromAppointmentStore();
                }

                if (e.NewValue is INotifyCollectionChanged newAppointments)
                {
                    newAppointments.CollectionChanged += control.AppointmentSourceCollectionChanged;
                    control.SyncAndRenderAppointments();
                }
            }
        }

        private void AppointmentSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                var newItems = (IEnumerable<Appointment>)e.NewItems;
                ClearAppointmentsFromAppointmentStore((IEnumerable<Appointment>)e.OldItems);
                SyncAppointmentsInAppointmentsStore(newItems);
                appointmentRenderingCanvas.Render(newItems);
            }
        }

        private void AppointmentGroupResourceChanged(object sender, GroupResourceChangedEventArgs e)
        {
            var appointment = (Appointment)sender;
            if (e.OldValue is not null && appointmentStore.TryGetValue(e.OldValue.Id, out GroupResource group))
            {
                group.Appointments.Remove(appointment);
            }

            if (appointmentStore.TryGetValue(e.NewValue.Id, out group))
            {
                group.Appointments.Add(appointment);
                appointmentRenderingCanvas.MeasureHeight(new[] { appointment });
            }
            else
            {
                appointment.Hide();
            }
        }

        private static void OnExtendedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            if (control.IsLoaded && control.InvalidateChildControlsArea())
            {
                control.rulerGrid.Render();
                control.appointmentRenderingCanvas.MeasureHeight(control.AppointmentSource);
            }
        }

        private static void OnTimeLineZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            if (control.IsLoaded)
            {
                control.InvalidateChildControlsArea();
                control.appointmentRenderingCanvas.MeasureWidth(control.AppointmentSource);
            }
        }

        private static void OnScheduleDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            if (control.IsLoaded)
            {
                if (control.ViewRange <= 0)
                {
                    d.CoerceValue(StartDateProperty);
                    d.CoerceValue(EndDateProperty);
                    return;
                }

                control.InvalidateChildControlsArea();
                control.dateHeader.ReArrangeHeaders();
                control.appointmentRenderingCanvas.Render(control.AppointmentSource.ToList());
            }
        }

        private static void OnTimeLineProvidersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            if (control.IsLoaded)
            {
                if (e.OldValue is INotifyCollectionChanged oldCollection)
                {
                    oldCollection.CollectionChanged -= control.TimeLineProvidersCollectionChanged;
                }

                if (e.NewValue is INotifyCollectionChanged newCollection)
                {
                    newCollection.CollectionChanged += control.TimeLineProvidersCollectionChanged;
                    control.timerulerPanel.Render();
                }
            }
        }

        private void TimeLineProvidersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => timerulerPanel?.Render();

        private static void OnTimeLineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ScheduleControl)d;
            if (control.IsLoaded)
            {
                control.rulerGrid.Render();
                control.timeLineHeader.Render();
            }
        }
        private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (IsLoaded && e.Source is ScrollViewer)
            {
                ScrollChanged?.Invoke(this, e);
                groupContainerScrollViewer?.ScrollToVerticalOffset(e.VerticalOffset);
            }
        }

        private void ScheduleControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsLoaded)
            {
                InvalidateChildControlsArea();
            }
        }

        private void ScheduleControlLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ScheduleControlLoaded;

            PrepareScheduleControl();
            SyncGroupsInAppointmentStore(GroupBy);
            SyncAndRenderAppointments();
        }

        private static object CoerceScheduleDates(DependencyObject d, object baseValue)
        {
            var control = (ScheduleControl)d;
            var date = (DateTime)baseValue;
            return date.Equals(default(DateTime)) || control.EndDate < control.StartDate ? control.StartDate : date;
        }

        private void SyncAndRenderAppointments()
        {
            SyncAppointmentsInAppointmentsStore(AppointmentSource);
            appointmentRenderingCanvas.Render(AppointmentSource);
        }
        private void PrepareScheduleControl()
        {
            GetScrollbarSize();
            FindAppointmentRenderingCanvas();
            InvalidateChildControlsArea();
            dateHeader.ReArrangeHeaders();

            (GetTemplateChild("PART_HeaderSectionRightGapMask") as Border).Width = scrollBarSpace;
            (GetTemplateChild("PART_HeaderSectionBottomGapMask") as Border).Height = scrollBarSpace;
        }

        private void SyncAppointmentsInAppointmentsStore(IEnumerable<Appointment> newItems)
        {
            if (!newItems.IsNullOrEmpty())
            {
                foreach (var item in newItems.GroupBy(a => a.Group.Id))
                {
                    if (appointmentStore.TryGetValue(item.Key, out GroupResource group))
                    {
                        group.Appointments.AddRange(item);
                    }
                }
            }
        }

        private void ClearAppointmentsFromAppointmentStore()
        {
            foreach (var group in appointmentStore)
            {
                group.Value.Appointments.Clear();
            }
        }

        private void ClearAppointmentsFromAppointmentStore(IEnumerable<Appointment> appointments)
        {
            foreach (var appointment in appointments.GroupBy(a => a.Group.Id))
            {
                if (appointmentStore.TryGetValue(appointment.Key, out GroupResource group))
                {
                    group.Appointments.Clear();
                }
            }
        }

        private void SyncGroupsInAppointmentStore(IEnumerable groups)
        {
            if (!groups.IsNullOrEmpty())
            {
                int order = 0;
                foreach (GroupResource group in groups)
                {
                    group.Order = order++;
                    appointmentStore.Add(group.Id, group);
                }
            }
        }

        private void GetScrollbarSize()
        {
            var scrollbars = new List<ScrollBar>();
            groupContainerScrollViewer = groupByContainer.GetChildOfType<ScrollViewer>();
            schedulerScrollViewer.GetChildOfType<ScrollBar>(ref scrollbars, level: 2);
            scrollBarSpace = scrollbars.ElementAt(0).ActualWidth;
        }
        private void FindAppointmentRenderingCanvas() =>
            appointmentRenderingCanvas = appointmentContainer.GetChildOfType<AppointmentRenderingCanvas>();

        private void HandleEvents()
        {
            Loaded += ScheduleControlLoaded;
            SizeChanged += ScheduleControlSizeChanged;
            schedulerScrollViewer.ScrollChanged += ScrollViewerScrollChanged;
            Appointment.GroupResourceChanged += AppointmentGroupResourceChanged;
        }

        private void UnHandleEvents()
        {
            SizeChanged -= ScheduleControlSizeChanged;
            schedulerScrollViewer.ScrollChanged -= ScrollViewerScrollChanged;
            if (AppointmentSource is not null)
            {
                AppointmentSource.CollectionChanged -= AppointmentSourceCollectionChanged;
            }

            if (GroupBy is not null)
            {
                GroupBy.CollectionChanged -= GroupByCollectionChanged;
            }

            if (TimeLineProviders is not null)
            {
                TimeLineProviders.CollectionChanged -= TimeLineProvidersCollectionChanged;
            }
        }

        private bool InvalidateChildControlsArea()
        {
            CalculateViewPortSize();
            CalculateRequiredAreaSize();
            if (contentSection.RenderSize != requiredViewPortArea)
            {
                contentSection.Width = requiredViewPortArea.Width;
                headerSection.Width = requiredViewPortArea.Width;
                contentSection.Height = requiredViewPortArea.Height;
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
            var requiredHeight = (GroupBy?.Count(g => g.Visibility == Visibility.Visible) ?? 0) * (int)this.ExtendedMode;
            requiredViewPortArea.Height = requiredHeight < viewPortArea.Height ? viewPortArea.Height : requiredHeight;
            requiredViewPortArea.Width = (TimeLineZoom switch
            {
                TimeLineZoom.Twelve => viewPortArea.Width * 2,
                TimeLineZoom.TwentyFour => viewPortArea.Width,
                TimeLineZoom.FortyEight => viewPortArea.Width / 2,
                _ => throw new NotImplementedException(),
            }) * ViewRange;
        }
    }
}
