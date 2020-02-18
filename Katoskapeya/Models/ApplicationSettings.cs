using System;
using System.Xml.Serialization;

namespace Kataskopeya.Models
{
    [Serializable()]
    [XmlRoot("settings")]
    public class ApplicationSettings
    {
        [XmlElement("DurationOfRecordedVideoChunk")]
        public int DurationOfRecordedVideoChunk { get; set; }
    }
}
