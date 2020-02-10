using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kataskopeya.Views;
using System;
using System.Windows.Input;

namespace Kataskopeya.ViewModels
{
    public class MenuViewModel : ObservableObject, IDisposable
    {
        public MenuViewModel()
        {
            OpenArchiveWindow = new RelayCommand(MoveToArchive);
            OpenCamerasWindow = new RelayCommand(MoveToCameras);
        }

        public Action CloseAction { get; set; }

        public ICommand OpenArchiveWindow { get; private set; }

        public ICommand OpenCamerasWindow { get; private set; }

        public void MoveToArchive()
        {
            var archive = new ArchiveView();
            archive.Show();
            CloseAction();
        }

        public void MoveToCameras()
        {
            var cameras = new CamerasMainView();
            cameras.Show();
            CloseAction();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
