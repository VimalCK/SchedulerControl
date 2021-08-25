using Scheduler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            DataContext = this;
            edt.SelectedDate = DateTime.Today.AddDays(7);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sc.TimeLineColor == Brushes.LightGray)
            {
                sc.TimeLineColor = Brushes.Red;
            }
            else if (sc.TimeLineColor == Brushes.Red)
            {
                sc.TimeLineColor = Brushes.Green;
            }
            else if (sc.TimeLineColor == Brushes.Green)
            {
                sc.TimeLineColor = Brushes.Black;
            }
            else
            {
                sc.TimeLineColor = Brushes.LightGray;
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            sc.IsExtendedMode = !sc.IsExtendedMode;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            switch (sc.TimeLineZoom)
            {
                case Scheduler.TimeLineZoom.Twelve:
                    sc.TimeLineZoom = Scheduler.TimeLineZoom.TwentyFour;
                    break;
                case Scheduler.TimeLineZoom.TwentyFour:
                    sc.TimeLineZoom = Scheduler.TimeLineZoom.FortyEight;
                    break;
                case Scheduler.TimeLineZoom.FortyEight:
                    sc.TimeLineZoom = Scheduler.TimeLineZoom.Twelve;
                    break;
                default:
                    break;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            sc.TimeLineProviders.Add(new TimeRuler { Color = Brushes.Blue, Time = "-01:00" });
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            sc.TimeLineProviders.RemoveAt(sc.TimeLineProviders.Count - 1);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var chars = new List<char>();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                chars.Add(c);
            }

            var items = new List<IAppointment>();
            for (int index = 0, iteration = 0; index < 5; index++)
            {
                items.Add(new Appointment()
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Description = "Appointment " + index,
                    Group = "AAA"//chars[iteration].ToString() + chars[iteration].ToString() + chars[index].ToString()
                }) ;

                if (index == 25 && iteration < 2)
                {
                    index = -1;
                    iteration++;
                }
            }

            sc.AppointmentSource = new ObservableCollection<IAppointment>(items);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                sc.AppointmentSource.Add(new Appointment()
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Description = "Appointment " + i,
                    Group = $"AAA{i}"
                });
            }
        }
    }
}
