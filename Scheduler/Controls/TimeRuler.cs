using System;
using System.Linq;
using System.Windows.Media;

namespace Scheduler
{
    public sealed class TimeRuler
    {
        private string time;
        private int hour;
        private int minute;

        internal int Hour => hour;
        internal int Minute => minute;
        public Brush Color { get; set; }
        public double Thickness { get; set; } = 1.5;
        public string Time
        {
            get => time;
            set
            {
                time = value;
                hour = 0;
                minute = 0;

                if (!string.IsNullOrEmpty(time))
                {
                    var values = time.Split(':').Select(v => int.Parse(v));
                    if (values.Count() != 2)
                    {
                        throw new Exception("Invalid Time format");
                    }

                    switch ((values.ElementAt(0), values.ElementAt(1)))
                    {
                        case (int a, int b) when a < 0 && a > 23:
                            throw new Exception("Hour specified in Time is not valid");
                        case (int a, int b) when b < 0 && b > 59:
                            throw new Exception("Minute specified in Time is not valid");
                        default:
                            hour = values.ElementAt(0);
                            minute = Math.Abs(values.ElementAt(1));
                            break;
                    }
                }
            }
        }

    }
}
