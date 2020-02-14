using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kataskopeya.ViewModels
{
    public class VideoPlayerViewModel : ObservableObject, IDisposable
    {
        private string _filename;
        private MediaElement _mediaElement;

        public VideoPlayerViewModel()
        {
            PreviousWindowCommand = new RelayCommand(GetToPreviousWindow);
            StopVideoCommand = new RelayCommand(StopVideo);
            StartVideoCommand = new RelayCommand(StartVideo);
        }

        public string Filename
        {
            get { return _filename; }
            set { Set(ref _filename, value); }
        }

        public MediaElement Player
        {
            get { return _mediaElement; }
            set { Set(ref _mediaElement, value); }
        }

        public ICommand PreviousWindowCommand { get; set; }

        public ICommand StopVideoCommand { get; set; }

        public ICommand StartVideoCommand { get; set; }

        public Action CloseAction { get; set; }

        private void GetToPreviousWindow()
        {
            CloseAction();
            Player.Close();
        }

        private void StartVideo()
        {
            Player.Play();
        }

        private void StopVideo()
        {
            Player.Stop();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
