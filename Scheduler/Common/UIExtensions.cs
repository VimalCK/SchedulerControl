using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Scheduler
{
    public static class UIExtensions
    {
        private static double pixelsPerDpi;
        private static Typeface typeface;
        private static CultureInfo cultureInfo;

        public static CultureInfo CultureInfo
        {
            get
            {
                if (cultureInfo == null)
                {
                    cultureInfo = new CultureInfo("en-US");
                }

                return cultureInfo;
            }
        }

        public static Typeface Typeface
        {
            get
            {
                if (typeface == null)
                {
                    typeface = new Typeface("Arial");
                }

                return typeface;
            }
        }

        public static double GetPixelsPerDpi(Visual visual)
        {
            if (pixelsPerDpi.Equals(0))
            {
                pixelsPerDpi = VisualTreeHelper.GetDpi(visual).PixelsPerDip;
            }

            return pixelsPerDpi;
        }

        public static FormattedText GetFormattedText(Visual visual, string value)
        {
            return new FormattedText(value, UIExtensions.CultureInfo, FlowDirection.LeftToRight,
                UIExtensions.Typeface, 10D, Brushes.Gray, UIExtensions.GetPixelsPerDpi(visual));
        }
    }
}
