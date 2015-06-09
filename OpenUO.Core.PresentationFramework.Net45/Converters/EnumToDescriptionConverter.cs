using System;
using System.ComponentModel;
using System.Windows.Data;

namespace OpenUO.Core.PresentationOpenUO.Core.Converters
{
    public sealed class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null || !(value is Enum))
            {
                return null;
            }

            var en = (Enum)value;

            return en.GetAttribute<DescriptionAttribute>().Description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}