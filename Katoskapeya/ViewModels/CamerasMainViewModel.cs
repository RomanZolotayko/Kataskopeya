using AForge.Imaging.Filters;
using AForge.Video;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kataskopeya.Extensions;
using Kataskopeya.Services;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Kataskopeya.ViewModels
{
    public class CamerasMainViewModel : ObservableObject, IDisposable
    {
        private BitmapImage _image;
        private IVideoSource _videoSource;
        private bool _original;
        private bool _grayscale;
        private byte[] _monitoringImage;
        private string _username;
        private int _fpsIndexer;

        private readonly VideoRecordingService _videoRecordingService;
        private readonly MotionDetectionService _motionDetectionService;
        private readonly FaceAnalyzerService _faceAnalyzerService;

        public CamerasMainViewModel()
        {
            _videoRecordingService = new VideoRecordingService();
            _motionDetectionService = new MotionDetectionService();
            _faceAnalyzerService = new FaceAnalyzerService();
            SetUpRecordingEngine();
            StartSourceCommand = new RelayCommand(StartCamera);
            StopSourceCommand = new RelayCommand(StopCamera);
            MakeGrayscale = new RelayCommand(ApplyGrayscale);
            MakeOriginal = new RelayCommand(ApplyOriginal);
            StopRecordVideoSource = new RelayCommand(StopRecord);
            IpCameraUrl = "http://192.168.128.82:8080/video";
            _fpsIndexer = 0;
        }

        public byte[] MonitoringImage
        {
            get { return _monitoringImage; }
            set { Set(ref _monitoringImage, value); }
        }

        public string Username
        {
            get { return _username; }
            set { Set(ref _username, value); }
        }

        public BitmapImage Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }

        public bool Grayscaled
        {
            get { return _grayscale; }
            set { Set(ref _grayscale, value); }
        }

        public bool Original
        {
            get { return _original; }
            set { Set(ref _original, value); }
        }

        public string IpCameraUrl { get; set; }

        public ICommand StartSourceCommand { get; private set; }

        public ICommand StopSourceCommand { get; private set; }

        public ICommand MakeGrayscale { get; private set; }

        public ICommand MakeOriginal { get; private set; }

        public ICommand StopRecordVideoSource { get; private set; }

        private void ApplyGrayscale()
        {
            if (Grayscaled)
            {
                Grayscaled = false;
            }
            else
            {
                Grayscaled = true;
            }
        }

        private void ApplyOriginal()
        {
            if (Original)
            {
                Original = false;
            }
            else
            {
                Original = true;
            }
        }

        private void StartCamera()
        {
            _videoSource = new MJPEGStream(IpCameraUrl);
            _videoSource.NewFrame += CaptureFace_Frame;
            _videoSource.Start();
        }

        private void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= CaptureFace_Frame;
            }
            Image = null;
        }

        private void SetUpRecordingEngine()
        {
            _videoRecordingService.SetUpRecordingEngine(480, 360);
        }

        private void StopRecord()
        {
            _videoRecordingService.StopVideoRecording();
        }

        private void CaptureFace_Frame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                BitmapImage bitmapImage;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    if (Grayscaled)
                    {
                        using (var grayscaledBitmap = Grayscale.CommonAlgorithms.BT709.Apply(bitmap))
                        {

                            bitmapImage = grayscaledBitmap.ToBitmapImage();
                        }
                    }
                    else
                    {
                        //_motionDetectionService.Detect(bitmap);
                        //_faceAnalyzerService.Analyze(bitmap, _fpsIndexer);
                        //_videoRecordingService.StartVideoRecording(bitmap);
                        bitmapImage = bitmap.ToBitmapImage();
                        _fpsIndexer++;
                    }
                }

                bitmapImage.Freeze();
                Dispatcher.CurrentDispatcher.Invoke(() => Image = bitmapImage);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCamera();
            }
        }

        public void Dispose()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
            }
        }


    }
}
