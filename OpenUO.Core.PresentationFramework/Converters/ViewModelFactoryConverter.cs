using System;
using System.Windows.Data;
using OpenUO.Core.PresentationFramework.ComponentModel;

namespace OpenUO.Core.PresentationFramework.Converters
{
    public sealed class ViewModelFactoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ViewModelLocator && parameter is Type)
            {
                ViewModelLocator factory = (ViewModelLocator)value;

                if (factory != null)
                {
                    return factory.Get((Type)parameter);
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
