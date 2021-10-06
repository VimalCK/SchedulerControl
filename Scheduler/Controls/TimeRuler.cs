using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace Scheduler
{
    public readonly struct TimeRuler
    {
        private readonly TimeSpan times;
        public Brush Color { get; init; }
        public double Thickness { get; init; } = 1.5;

        [TypeConverter(typeof(TimeSpanConverter))]
        public TimeSpan Time
        {
            get => times;
            init
            {
                times = value;
            }
        }

    }
}
