using AForge.Video;
using GalaSoft.MvvmLight;
using Kataskopeya.Commands;
using Kataskopeya.Services;
using Kataskopeya.ViewModels;
using Kataskopeya.Views;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Kataskopeya.Models
{
    public class MonitoringImage : ObservableObject
    {
        private ICommand _openCameraDetailsCommand;
        private BitmapImage _image;

        public MonitoringImage(string source, int width, int height)
        {
            Url = source;
            GridWidth = width;
            GridHeight = height;
            IsRecordSetupNeed = true;
        }

        public MonitoringImage()
        {

        }
        public int GridWidth { get; set; }

        public int GridHeight { get; set; }

        public string CameraId { get; set; }

        public string CameraName { get; set; }

        public bool IsRecordSetupNeed { get; set; }

        public VideoRecordingService VideoRecordingService { get; set; }

        public string Url { get; set; }

        public BitmapImage Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }

        public IVideoSource VideoSource { get; set; }

        public ICommand OpenCameraDetailsCommand
        {
            get
            {
                return _openCameraDetailsCommand ?? (_openCameraDetailsCommand = new BaseCommandHandler(param => OpenCameraDetailsHandler(param), true));
            }
        }

        public void OpenCameraDetailsHandler(object param)
        {
            VideoSource.SignalToStop();
            var cameraDetail = new CameraDetailsView();
            cameraDetail.DataContext = new CameraDetailsViewModel(param as string)
            {
                CloseAction = ((CameraDetailsViewModel)cameraDetail.DataContext).CloseAction
            };

            cameraDetail.ShowDialog();
            VideoSource.Start();
        }
    }
}
