using Bibliotech.Model.Entities.Enums;
using EnumsNET;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Bibliotech.Converters
{
    public class TypeUserValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TypeUser typeUser = (TypeUser)value;
            return typeUser.AsString(EnumFormat.Description);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
