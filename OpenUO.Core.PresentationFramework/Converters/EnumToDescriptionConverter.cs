using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;

namespace OpenUO.Core.PresentationFramework.Converters
{
    public sealed class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null || !(value is Enum))
                return null;

            Enum en = (Enum)value;

            return en.GetAttribute<DescriptionAttribute>().Description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
