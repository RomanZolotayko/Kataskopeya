using System;

namespace Kataskopeya.Extensions
{
    public static class StringExtensions
    {
        private static string _fileNameSeparator = "_";

        public static string GetVideoFileName(this string cameraName)
        {
            return cameraName + _fileNameSeparator + "(" + DateTime.Now.ToString("G").Replace(".", "-").Replace(":", "_") + ")";
        }

        public static DateTime GetDateFromFileName(this string filename)
        {
            var dateString = filename.Split('(', ')')[1].Replace("_", ":").Replace("-", ".");

            return DateTime.Parse(dateString);
        }
    }
}
