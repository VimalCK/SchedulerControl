using System;
using System.Collections.Generic;
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
            //sdt.SelectedDate = DateTime.Now.Date;
            //edt.SelectedDate = DateTime.Now.Date.AddDays(2);
            this.DataContext = this;
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
    }
}
