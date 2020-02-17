using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Kataskopeya.ViewModels
{
    public class VideoPlayerViewModel : ObservableObject, IDisposable
    {
        private string _filename;
        private MediaElement _mediaElement;
        private double _videoDuration;
        private double _videoSpeedRate = 1;
        private DispatcherTimer timerVideoPlayback;
        private double _currentVideoPosition = 0;
        private bool _isVideoPaused;
        private double _sliderMarkSpeed = 0.1;

        public VideoPlayerViewModel()
        {
            PreviousWindowCommand = new RelayCommand(GetToPreviousWindow);
            StopVideoCommand = new RelayCommand(StopVideo);
            StartVideoCommand = new RelayCommand(StartVideo);
            AccelarateVideoCommand = new RelayCommand(AccelerateVideo);
            SlowVideoCommand = new RelayCommand(SlowVideo);
            MoveVideoBackCommand = new RelayCommand(MoveVideoBack);
            MoveVideoForwardCommand = new RelayCommand(MoveVideoForward);

            timerVideoPlayback = new DispatcherTimer();
            timerVideoPlayback.Interval = TimeSpan.FromSeconds(_sliderMarkSpeed);
            timerVideoPlayback.Tick += HandlerTimerTick;
            timerVideoPlayback.Start();
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

        public double VideoDuration
        {
            get { return _videoDuration; }
            set { Set(ref _videoDuration, value); }
        }

        public double CurrentVideoPosition
        {
            get { return _currentVideoPosition; }
            set { Set(ref _currentVideoPosition, value); }
        }

        public ICommand PreviousWindowCommand { get; set; }

        public ICommand StopVideoCommand { get; set; }

        public ICommand StartVideoCommand { get; set; }

        public ICommand AccelarateVideoCommand { get; set; }

        public ICommand SlowVideoCommand { get; set; }

        public ICommand MoveVideoForwardCommand { get; set; }

        public ICommand MoveVideoBackCommand { get; set; }

        public Action CloseAction { get; set; }

        private void GetToPreviousWindow()
        {
            CloseAction();
            Player.Close();
        }

        private void StartVideo()
        {
            _isVideoPaused = false;
            Player.Play();
        }

        private void StopVideo()
        {
            _isVideoPaused = true;
            Player.Stop();
        }

        public void GetMediaDuration(object sender, EventArgs e)
        {
            var media = sender as MediaElement;
            var secondsDuration = media.NaturalDuration.TimeSpan.TotalSeconds;

            VideoDuration = Math.Round(secondsDuration, 2);
        }

        public void HandlerTimerTick(object sender, EventArgs e)
        {
            if (!_isVideoPaused)
            {
                CurrentVideoPosition += 0.1;
            }
        }

        public void SliderValueChangedHandler(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newSliderValue = TimeSpan.FromSeconds(e.NewValue);
            if (e.NewValue > 1)
            {
                Player.Position = newSliderValue;
            }
        }

        public void SliderLostMouseCaptureHandler(object sender, MouseEventArgs e)
        {
            var source = e.Source as Slider;
            var sliderValue = source.Value;
            Player.Position = TimeSpan.FromSeconds(sliderValue);
            _isVideoPaused = false;
        }

        private void AccelerateVideo()
        {
            _videoSpeedRate *= 2;
            timerVideoPlayback.Interval = TimeSpan.FromSeconds(_sliderMarkSpeed / 2);
            Player.SpeedRatio = _videoSpeedRate;
        }

        private void SlowVideo()
        {
            _videoSpeedRate /= 2;
            timerVideoPlayback.Interval = TimeSpan.FromSeconds(_sliderMarkSpeed * 2);
            Player.SpeedRatio = _videoSpeedRate;
        }

        private void MoveVideoForward()
        {
            CurrentVideoPosition += 10;
            Player.Position = TimeSpan.FromSeconds(CurrentVideoPosition);
        }

        private void MoveVideoBack()
        {
            if (CurrentVideoPosition > 10)
            {
                CurrentVideoPosition -= 10;
            }
            else
            {
                CurrentVideoPosition = 0;
            }

            Player.Position = TimeSpan.FromSeconds(CurrentVideoPosition);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
