using System;
using System.Globalization;
using System.Windows.Data;

namespace Bibliotech.Converters
{
    public class DateTimeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = DateTime.TryParseExact(value.ToString(), "dd/MM/yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime dt);

            if (result == false)
            {
                result = DateTime.TryParseExact(value.ToString(), "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out dt);
            }

            return result ? dt.ToString("dd/MM/yyyy") : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
