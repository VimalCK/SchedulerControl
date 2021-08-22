using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Scheduler
{
    [ValueConversion(typeof(Enum), typeof(int))]
    internal sealed class EnumToValueConverter : MarkupExtension, IValueConverter
    {
        private static EnumToValueConverter converter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Enum.TryParse(value.GetType(), value?.ToString(), out object result))
            {
                return System.Convert.ChangeType(result, targetType);
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => converter ?? (converter = new EnumToValueConverter());

    }
}
