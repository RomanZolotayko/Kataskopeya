using System;

namespace Kataskopeya.Extensions
{
    public static class StringExtensions
    {
        private static string _fileNameSeparator = "_";

        public static string GetVideoFileName(this string cameraName)
        {
            return cameraName + _fileNameSeparator + DateTime.Now.TimeOfDay.ToString().Replace(".", "_").Replace(":", "_");
        }
    }
}
