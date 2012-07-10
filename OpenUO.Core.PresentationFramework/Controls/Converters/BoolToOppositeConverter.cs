using System;
using System.Windows.Data;

namespace OpenUO.Core.PresentationFramework.Converters
{
    public class BoolToOppositeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return !(bool)value;            
        }

        #endregion
    }
}
