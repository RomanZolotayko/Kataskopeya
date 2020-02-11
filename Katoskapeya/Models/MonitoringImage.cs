using GalaSoft.MvvmLight;
using System.Windows.Media.Imaging;

namespace Kataskopeya.Models
{
    public class MonitoringImage : ObservableObject
    {
        public MonitoringImage(int id, string source)
        {
            CameraId = id;
            Source = source;
        }

        private BitmapImage _image;

        public int CameraId { get; set; }

        public string Source { get; set; }

        public BitmapImage Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }


    }
}
