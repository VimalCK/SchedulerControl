﻿using System.Windows;
using System.Windows.Media;

namespace Scheduler
{
    public sealed class TimeRuler : Freezable
    {
        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public string Time
        {
            get => (string)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(string), typeof(TimeRuler), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(TimeRuler), new PropertyMetadata(Brushes.Red));

        protected override Freezable CreateInstanceCore() => new TimeRuler();
    }
}