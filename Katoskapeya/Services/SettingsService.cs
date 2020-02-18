using Kataskopeya.Common.Constants;
using Kataskopeya.Models;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Kataskopeya.Services
{
    public static class SettingsService
    {
        private static ApplicationSettings _applicationSettings;
        private static XmlSerializer _xmlSerializer;
        private static string filepath = "settings.xml";

        static SettingsService()
        {
            _applicationSettings = new ApplicationSettings();
            _xmlSerializer = new XmlSerializer(typeof(ApplicationSettings));
            CheckIsSettingsFileExist();
            DeserializeSettings();
        }

        public static void SaveSettings(ApplicationSettings settings)
        {
            _applicationSettings = settings;

            using (var _writter = new StreamWriter(filepath))
            {
                _xmlSerializer.Serialize(_writter, _applicationSettings);
            }
        }

        private static void DeserializeSettings()
        {
            using (var stream = new StreamReader(filepath))
            {
                var settings = (ApplicationSettings)_xmlSerializer.Deserialize(stream);
                FillObjectByData(settings);
            }
        }

        private static void FillObjectByData(ApplicationSettings settings)
        {
            _applicationSettings.DurationOfRecordedVideoChunk = settings.DurationOfRecordedVideoChunk;
        }

        private static void CheckIsSettingsFileExist()
        {
            var directory = new DirectoryInfo(FileSystemPaths.DebugFolder);
            var files = directory.GetFiles("settings.xml");

            if (!files.Any())
            {
                using (var streamWritter = new StreamWriter(filepath))
                {
                    _xmlSerializer.Serialize(streamWritter, FillFileByDefaultData());
                }
            }
        }

        private static ApplicationSettings FillFileByDefaultData()
        {
            var settings = new ApplicationSettings
            {
                DurationOfRecordedVideoChunk = 15
            };

            return settings;
        }

        public static ApplicationSettings GetApplicationSettings()
        {
            return _applicationSettings;
        }
    }
}
