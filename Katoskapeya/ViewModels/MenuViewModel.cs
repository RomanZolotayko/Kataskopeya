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
            OpenArchiveWindowCommand = new RelayCommand(MoveToArchive);
            OpenCamerasWindowCommand = new RelayCommand(MoveToCameras);
            OpenSettingsWindowCommand = new RelayCommand(MoveToSettings);
        }

        public Action CloseAction { get; set; }

        public ICommand OpenArchiveWindowCommand { get; private set; }

        public ICommand OpenCamerasWindowCommand { get; private set; }

        public ICommand OpenSettingsWindowCommand { get; private set; }

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

        public void MoveToSettings()
        {
            var cameras = new SettingsView();
            cameras.Show();
            CloseAction();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
