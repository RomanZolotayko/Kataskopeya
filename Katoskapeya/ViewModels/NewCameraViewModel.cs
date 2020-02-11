using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kataskopeya.Views;
using System;
using System.Windows.Input;

namespace Kataskopeya.ViewModels
{
    public class NewCameraViewModel : ObservableObject, IDisposable
    {
        private string _newIpCameraUrl;

        public NewCameraViewModel()
        {
            AddNewCameraUrlCommand = new RelayCommand(AddNewCameraUrl);
        }

        public string NewIpCameraUrl
        {
            get { return _newIpCameraUrl; }
            set { Set(ref _newIpCameraUrl, value); }
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
