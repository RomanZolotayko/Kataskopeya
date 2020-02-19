using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kataskopeya.Views;
using System;
using System.Windows;
using System.Windows.Input;

namespace Kataskopeya.ViewModels
{
    public class ScanViewModel : ObservableObject, IDisposable
    {
        private string _username;
        private int _age;

        public ScanViewModel()
        {
            StartFaceScanCommand = new RelayCommand(StartFaceScan);
            PreviousWindowCommand = new RelayCommand(GetToPreviousWindow);
        }

        public string Username
        {
            get { return _username; }
            set { Set(ref _username, value); }
        }

        public int Age
        {
            get { return _age; }
            set { Set(ref _age, value); }
        }

        public Action CloseAction { get; set; }

        public ICommand StartFaceScanCommand { get; set; }

        public ICommand PreviousWindowCommand { get; set; }

        private void StartFaceScan()
        {
            if (string.IsNullOrEmpty(Username) || Username == "Enter your name")
            {
                MessageBox.Show("Please enter a valid name", "Exception", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var faceScanner = new FaceScannerView();
            ((FaceScannerViewModel)faceScanner.DataContext).Username = Username;
            ((FaceScannerViewModel)faceScanner.DataContext).Age = Age;
            faceScanner.ShowDialog();
        }

        public void GetToPreviousWindow()
        {
            var menu = new MenuView();
            menu.Show();
            CloseAction();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
