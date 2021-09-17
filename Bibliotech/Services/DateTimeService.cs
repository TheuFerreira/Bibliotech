using System;
using System.Globalization;

namespace Bibliotech.Services
{
    public class DateTimeService
    {
        public bool CheckIfIsDate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            bool result = DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime datetime);
            if (result == false)
            {
                return false;
            }

            return true;
        }

        public DateTime? ConvertString(string value)
        {
            DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime datetime);

            return datetime;
        }
    }
}
