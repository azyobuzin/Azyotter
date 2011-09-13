using System;
using System.Windows;
using System.Windows.Data;

namespace Azyobuzi.Azyotter.Views.Converters
{
    public class TypeToEnumValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Type)) return DependencyProperty.UnsetValue;

            return Enum.GetValues(value as Type);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
