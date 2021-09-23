using Scheduler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // TimeLineProviders.Add(new TimeRuler { Color = Brushes.Blue, Time = "-01:00" });
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //  TimeLineProviders.RemoveAt(TimeLineProviders.Count - 1);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var chars = new List<char>();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                chars.Add(c);
            }

            var list = new List<string>();
            for (int index = 0, iteration = 0; index < 5; index++)
            {
                list.Add(chars[iteration].ToString() + chars[iteration].ToString() + chars[index].ToString());
            }


        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < 20; i++)
            //{
            //    AppointmentSource.Add(new Appointment()
            //    {
            //        StartDate = DateTime.Now,
            //        EndDate = DateTime.Now,
            //        Description = "Appointment " + i,
            //        Group = $"AAA{i}"
            //    });
            //}
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

        public ICommand ExtendedModeCommand { get; set; }
        public ICommand TimeLineZoomCommand { get; set; }
        public ICommand TimeLineColorCommand { get; set; }
        public ICommand AddGroupHeadersCommand { get; set; }
        public ICommand RemoveGroupHeadersCommand { get; set; }
        public ICommand AddTimelineCommand { get; set; }
        public ICommand RemoveTimelineCommand { get; set; }
        public ICommand HideShowGroupCommand { get; set; }

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

            ExtendedModeCommand = new RelayCommand(ChangeExtendedMode);
            TimeLineZoomCommand = new RelayCommand(ChangeTimeLine);
            TimeLineColorCommand = new RelayCommand(ChangeTimeLineColor);
            AddGroupHeadersCommand = new RelayCommand(AddGroupHeaders);
            RemoveGroupHeadersCommand = new RelayCommand(RemoveGroupHeaders);
            AddTimelineCommand = new RelayCommand(AddTimeline);
            RemoveTimelineCommand = new RelayCommand(RemoveTimeline);
            LoadGroupResources();
            LoadTimelineProviders();
        }

        private void LoadTimelineProviders()
        {
            timelineProviders = new ObservableCollection<TimeRuler>();
            timelineProviders.Add(new TimeRuler { Color = Brushes.Red, Thickness = 2 });
            timelineProviders.Add(new TimeRuler { Color = Brushes.Purple, Thickness = 1, Time = "-02:00" });
            timelineProviders.Add(new TimeRuler { Color = Brushes.Green, Thickness = 2, Time = "23:00" });
        }

        private void RemoveTimeline(object obj)
        {
            TimelineProviders.RemoveAt(timelineProviders.Count - 1);
        }

        private void AddTimeline(object obj)
        {
            TimelineProviders.Add(new TimeRuler { Color = Brushes.Brown, Thickness = 1.5, Time = $"0{TimelineProviders.Count}:00" });
        }

        private void AddGroupHeaders(object obj)
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
                    Header = chars[random.Next(0, 25)].ToString() +
                    chars[random.Next(0, 25)].ToString() +
                    chars[random.Next(0, 25)].ToString()
                };

                if (!GroupResources.Contains(group))
                {
                    GroupResources.Add(group);
                    break;
                }

            } while (true);
        }

        private void RemoveGroupHeaders(object obj)
        {
            groupResources.RemoveAt(new Random().Next(0, GroupResources.Count - 1));
        }

        private void LoadGroupResources()
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
                    Header = chars[iteration].ToString() + chars[iteration].ToString() + chars[index].ToString()
                });
            }

            GroupResources = new ObservableCollection<GroupResource>(list);
        }

        private void ChangeTimeLineColor(object obj)
        {
            if (TimelineColor == Brushes.LightGray)
            {
                TimelineColor = Brushes.Red;
            }
            else if (TimelineColor == Brushes.Red)
            {
                TimelineColor = Brushes.Green;
            }
            else if (TimelineColor == Brushes.Green)
            {
                TimelineColor = Brushes.Black;
            }
            else
            {
                TimelineColor = Brushes.LightGray;
            }
        }

        private void ChangeTimeLine(object obj)
        {
            switch (TimeLineZoom)
            {
                case Scheduler.TimeLineZoom.Twelve:
                    TimeLineZoom = Scheduler.TimeLineZoom.TwentyFour;
                    break;
                case Scheduler.TimeLineZoom.TwentyFour:
                    TimeLineZoom = Scheduler.TimeLineZoom.FortyEight;
                    break;
                case Scheduler.TimeLineZoom.FortyEight:
                    TimeLineZoom = Scheduler.TimeLineZoom.Twelve;
                    break;
                default:
                    TimeLineZoom = Scheduler.TimeLineZoom.TwentyFour;
                    break;
            }
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
