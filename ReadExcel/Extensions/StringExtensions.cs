using System;
using System.Globalization;
using System.Text;

namespace ReadExcel.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveDiacritics(this string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string[] SplitValues(this string str)
        {
            return str.Split(new string[] { ",", " e ", ";", "/" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string TrimAllSpacies(this string str)
        {
            return str.Trim().Replace("  ", " ");
        }
    }
}
