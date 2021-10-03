using System;
using System.Linq;
using System.Windows.Media;

namespace Scheduler
{
    public readonly struct TimeRuler
    {
        private readonly string time;

        internal int Hour { get; init; }
        internal int Minute { get; init; }
        public Brush Color { get; init; }
        public double Thickness { get; init; } = 1.5;

        public string Time
        {
            get => time;
            init
            {
                time = value;
                Hour = 0;
                Minute = 0;

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
                            Hour = values.ElementAt(0);
                            Minute = Math.Abs(values.ElementAt(1));
                            break;
                    }
                }
            }
        }

    }
}
