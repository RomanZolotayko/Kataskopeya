using GalaSoft.MvvmLight;
using Kataskopeya.Commands;
using Kataskopeya.ViewModels;
using Kataskopeya.Views;
using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Kataskopeya.Models
{
    public class MonitoringImage : ObservableObject
    {
        private ICommand _openCameraDetailsCommand;

        public MonitoringImage(string source, int width, int height)
        {
            Url = source;
            Width = width;
            Height = height;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        private BitmapImage _image;

        public string Url { get; set; }

        public BitmapImage Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }

        public ICommand OpenCameraDetailsCommand
        {
            get
            {
                return _openCameraDetailsCommand ?? (_openCameraDetailsCommand = new BaseCommandHandler(param => OpenCameraDetailsHandler(param), true));
            }
        }

        public void OpenCameraDetailsHandler(object param)
        {
            var cameraDetail = new CameraDetailsView();
            cameraDetail.DataContext = new CameraDetailsViewModel(param as string)
            {
                CloseAction = ((CameraDetailsViewModel)cameraDetail.DataContext).CloseAction
            };
            cameraDetail.ShowDialog();
        }
    }
}
