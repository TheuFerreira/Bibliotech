using Bibliotech.Model.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Bibliotech.Converters
{
    public class AuthorsValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<Author> authors = value as List<Author>;

            string text = string.Empty;
            for (int i = 0; i < authors.Count; i++)
            {
                Author author = authors[i];
                text += author.Name;

                if (i < authors.Count - 1)
                {
                    text += ", ";
                }
            }

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
