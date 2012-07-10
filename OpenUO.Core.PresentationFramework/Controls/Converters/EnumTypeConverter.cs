using System;
using System.Windows.Data;

namespace OpenUO.Core.PresentationFramework.Converters
{
	public class EnumTypeConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return Enum.GetValues(value.GetType());
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
