using Scheduler;
using Scheduler.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class Test
    {

    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }



    public class MainViewModel : ViewModel
    {

        private DateTime startDate;
        private DateTime endDate;
        private ExtendedMode extendedMode;
        private Brush timelineColor;
        private TimeLineZoom timeLineZoom;
        private ObservableCollection<TimeRuler> timelineProviders;
        private ObservableCollection<GroupResource> groupResources;
        private ObservableCollection<Appointment> appointments;

        public ICommand ExtendedModeCommand { get; set; }
        public ICommand TimeLineZoomCommand { get; set; }
        public ICommand TimeLineColorCommand { get; set; }
        public ICommand LoadClearGroupResourcesCommand { get; set; }
        public ICommand AddGroupResourceCommand { get; set; }
        public ICommand RemoveGroupResourceCommand { get; set; }
        public ICommand AddTimelineCommand { get; set; }
        public ICommand RemoveTimelineCommand { get; set; }
        public ICommand LoadAppointmentsCommand { get; set; }
        public ICommand AddAppointmentCommand { get; set; }
        public ICommand RemoveAppointmentCommand { get; set; }
        public ICommand ChangeAppointmentTimeCommand { get; set; }
        public ICommand ChangeGroupCommand { get; set; }

        public ObservableCollection<Appointment> Appointments
        {
            get { return appointments; }
            set
            {
                appointments = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TimeRuler> TimelineProviders
        {
            get { return timelineProviders; }
            set
            {
                timelineProviders = value;
                OnPropertyChanged();
            }
        }

        public Brush TimelineColor
        {
            get { return timelineColor; }
            set
            {
                timelineColor = value;
                OnPropertyChanged();
            }
        }

        public TimeLineZoom TimeLineZoom
        {
            get { return timeLineZoom; }
            set
            {
                timeLineZoom = value;
                OnPropertyChanged();
            }
        }

        public ExtendedMode ExtendedMode
        {
            get { return extendedMode; }
            set
            {
                extendedMode = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                startDate = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<GroupResource> GroupResources
        {
            get => groupResources;
            set
            {
                groupResources = value;
                OnPropertyChanged();
            }
        }


        public MainViewModel()
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddDays(7);
            TimeLineZoom = TimeLineZoom.FortyEight;

            ExtendedModeCommand = new RelayCommand(ChangeExtendedMode);
            TimeLineZoomCommand = new RelayCommand(ChangeTimeLine);
            TimeLineColorCommand = new RelayCommand(ChangeTimeLineColor);
            LoadClearGroupResourcesCommand = new RelayCommand(LoadClearGroupResources);
            AddGroupResourceCommand = new RelayCommand(AddGroupResource);
            RemoveGroupResourceCommand = new RelayCommand(RemoveGroupResource);
            AddTimelineCommand = new RelayCommand(AddTimeline);
            RemoveTimelineCommand = new RelayCommand(RemoveTimeline);
            LoadAppointmentsCommand = new RelayCommand(LoadAppointments);
            AddAppointmentCommand = new RelayCommand(AddAppointments);
            RemoveAppointmentCommand = new RelayCommand(RemoveAppointments);
            ChangeAppointmentTimeCommand = new RelayCommand(ChangeAppointmentTime);
            ChangeGroupCommand = new RelayCommand(ChangeGroup);
            LoadClearGroupResources(null);
            LoadTimelineProviders();
        }

        private void ChangeGroup(object obj)
        {
            bool value = false;
            foreach (Appointment item in Appointments.Where(a => (a.Group as CustomGroup).Header == "AAA"))
            {
                item.Group = value ? GroupResources.OfType<CustomGroup>().First(g => g.Header == "AAH") :
                    GroupResources.OfType<CustomGroup>().First(g => g.Header == "AAI");
                value = !value;
                Task.Delay(50);
            }
        }

        private void ChangeAppointmentTime(object obj)
        {
            foreach (var item in Appointments.ToList())
            {
                if ((item.EndDateTime - item.StartDateTime).Hours > 3)
                {
                    item.EndDateTime = item.EndDateTime.AddHours(-2);
                }
                if ((item.EndDateTime - item.StartDateTime).Hours < 3)
                {
                    item.EndDateTime = item.EndDateTime.AddHours(3);
                }
            }
        }

        private void RemoveAppointments(object obj)
        {
            var appointments = Appointments.ToList();
            foreach (var item in Appointments.ToList())
            {
                Appointments.Remove(item);
                Task.Delay(50);
            }
        }

        private void AddAppointments(object obj)
        {
            int index = 0;
            foreach (var group in GroupResources)
            {
                var startDate = DateTime.Today.AddDays(7);
                var endDate = DateTime.Today.AddDays(7);
                var noOfFlights = new Random().Next(4, 6);
                var hour = new Random();
                for (int i = 0; i <= noOfFlights; i++)
                {
                    startDate = startDate.AddHours(hour.Next(1, 3));
                    endDate = startDate.AddHours(hour.Next(2, 4));
                    Appointments.Add(new Appointment(startDate, endDate, group)
                    {
                        Description = $"FL-{group.ToString()}-{i + 1} : {startDate.ToString("HH:mm")}-{endDate.ToString("HH:mm")}"
                    });

                    startDate = endDate;
                }

                index++;
            }
        }

        private void LoadAppointments(object value)
        {
            if (groupResources.IsNullOrEmpty()) { return; }

            int index = 0;
            var flightLegs = new List<Appointment>();
            foreach (CustomGroup group in GroupResources)
            {
                if (group.Header == "SPRAA" || group.Header == "SPRAE")
                {
                    continue;
                }

                var startDate = DateTime.Today;
                var endDate = DateTime.Today;
                var noOfFlights = new Random().Next(4, 20);
                var hour = new Random();
                for (int i = 0; i <= noOfFlights; i++)
                {
                    startDate = startDate.AddHours(hour.Next(1, 8));
                    endDate = startDate.AddHours(hour.Next(2, 5));
                    flightLegs.Add(new Appointment(startDate, endDate, group)
                    {
                        Description = $"FL-{group.ToString()}-{startDate.Day} : {startDate.ToString("HH:mm")}-{endDate.ToString("HH:mm")}"
                    });

                    startDate = endDate;
                    if (i == 4 || i == 6)
                    {
                        startDate = startDate.AddMinutes(i * 5);
                    }
                    else
                    {
                        startDate = startDate.AddHours(1);
                    }
                }

                index++;
            }

            Appointments = new ObservableCollection<Appointment>(flightLegs);
        }

        private void LoadTimelineProviders()
        {
            timelineProviders = new ObservableCollection<TimeRuler>();
            timelineProviders.Add(new TimeRuler { Color = Brushes.Red, Thickness = 2 });
            timelineProviders.Add(new TimeRuler { Color = Brushes.Purple, Thickness = 1, Time = new TimeSpan(-2, 0, 0) });
            timelineProviders.Add(new TimeRuler { Color = Brushes.Green, Thickness = 2, Time = new TimeSpan(23, 0, 0) });
        }

        private void RemoveTimeline(object obj)
        {
            TimelineProviders.RemoveAt(timelineProviders.Count - 1);
        }

        private void AddTimeline(object obj)
        {
            TimelineProviders.Add(new TimeRuler { Color = Brushes.Brown, Thickness = 1.5, Time = new TimeSpan(TimelineProviders.Count, 0, 0) });
        }



        private void AddGroupResource(object obj)
        {
            var chars = new List<char>();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                chars.Add(c);
            }

            do
            {
                var random = new Random();
                var group = new CustomGroup
                {
                    Header = "SPK" + chars[random.Next(0, 25)].ToString() + chars[random.Next(0, 25)].ToString()
                };

                if (!GroupResources.Contains(group))
                {
                    GroupResources.Insert(random.Next(0, GroupResources.Count - 1), group);
                    break;
                }

            } while (true);
        }

        private void RemoveGroupResource(object obj)
        {
            groupResources.RemoveAt(new Random().Next(0, GroupResources.Count - 1));
        }

        private void LoadClearGroupResources(object value)
        {
            if (GroupResources is null)
            {
                var chars = new List<char>();
                for (char c = 'A'; c <= 'Z'; c++)
                {
                    chars.Add(c);
                }

                var list = new List<GroupResource>();
                for (int index = 0, iteration = 0; index < 26; index++)
                {
                    list.Add(new CustomGroup
                    {
                        Header = "SPR" + chars[iteration].ToString() + chars[index].ToString()
                    });

                    if (index == 25)
                    {
                        index = 0;
                        iteration++;
                        if (iteration == 10)
                        {
                            break;
                        }
                    }
                }

                GroupResources = new ObservableCollection<GroupResource>(list);
                LoadAppointments(value);
            }
            else
            {
                GroupResources = null;
            }
        }

        private void ChangeTimeLineColor(object obj)
        {
            TimelineColor = timelineColor switch
            {
                var b when b == Brushes.LightGray => Brushes.Red,
                var b when b == Brushes.Red => Brushes.Green,
                var b when b == Brushes.Green => Brushes.Black,
                var b when b == Brushes.Black => Brushes.LightGray,
                _ => Brushes.Red,
            };
        }

        private void ChangeTimeLine(object obj)
        {
            TimeLineZoom = timeLineZoom switch
            {
                Scheduler.TimeLineZoom.Twelve => Scheduler.TimeLineZoom.TwentyFour,
                Scheduler.TimeLineZoom.TwentyFour => Scheduler.TimeLineZoom.FortyEight,
                Scheduler.TimeLineZoom.FortyEight => Scheduler.TimeLineZoom.Twelve,
                _ => throw new Exception(),
            };
        }

        private void ChangeExtendedMode(object obj)
        {
            ExtendedMode = extendedMode == ExtendedMode.Zoom ? ExtendedMode.Normal : ExtendedMode.Zoom;
        }
    }


    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<object> _execute;


        public RelayCommand(Action<object> execute)
        {
            this._execute = execute;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    public class CustomGroup : GroupResource
    {
        private string header;

        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                OnPropertyChanged();
            }
        }

        public CustomGroup()
        {
        }

        public override int GetHashCode()
        {
            return header.GetHashCode();
        }

        public override string ToString()
        {
            return header;
        }
    }

}
