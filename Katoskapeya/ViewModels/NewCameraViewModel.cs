using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Windows.Input;

namespace Kataskopeya.ViewModels
{
    public class NewCameraViewModel : ObservableObject, IDisposable
    {
        private string _ipCameraUrl;
        private string _cameraName;

        public NewCameraViewModel()
        {
            AddNewCameraUrlCommand = new RelayCommand(AddNewCameraUrl);
        }

        public string IpCameraUrl
        {
            get { return _ipCameraUrl; }
            set { Set(ref _ipCameraUrl, value); }
        }

        public string CameraName
        {
            get { return _cameraName; }
            set { Set(ref _cameraName, value); }
        }

        public Action CloseAction { get; set; }

        public ICommand AddNewCameraUrlCommand { get; set; }

        public void AddNewCameraUrl()
        {
            CloseAction();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
