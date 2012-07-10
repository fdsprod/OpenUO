using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace OpenUO.Core.PresentationFramework.Converters
{
    public class DivideByTwoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return 0;

            string v = value.ToString();
            double outValue;

            if (!double.TryParse(v, out outValue))
            {
                return 0;
            }

            return outValue / 2.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
