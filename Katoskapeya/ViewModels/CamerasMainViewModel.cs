using AForge.Imaging.Filters;
using AForge.Video;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kataskopeya.Extensions;
using Kataskopeya.Models;
using Kataskopeya.Services;
using Kataskopeya.Views;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Kataskopeya.ViewModels
{
    public class CamerasMainViewModel : ObservableObject, IDisposable
    {
        private BitmapImage _image;
        private ObservableCollection<MonitoringImage> _monitoringImages;
        private IVideoSource _videoSource;
        private bool _original;
        private bool _grayscale;
        private string _username;
        private int _fpsIndexer;
        private ObservableCollection<CameraScreen> _ipCameraUrls;
        private readonly VideoRecordingService _videoRecordingService;
        private readonly MotionDetectionService _motionDetectionService;
        private readonly FaceAnalyzerService _faceAnalyzerService;
        private int _internalIndexer;

        public CamerasMainViewModel()
        {
            _videoRecordingService = new VideoRecordingService();
            _motionDetectionService = new MotionDetectionService();
            _faceAnalyzerService = new FaceAnalyzerService();
            SetUpRecordingEngine();
            StartSourceCommand = new RelayCommand(StartCamera);
            StopSourceCommand = new RelayCommand(StopCamera);
            MakeGrayscaleCommand = new RelayCommand(ApplyGrayscale);
            MakeOriginalCommand = new RelayCommand(ApplyOriginal);
            PreviousWindowCommand = new RelayCommand(GetToPreviousWindow);
            StopRecordVideoSourceCommand = new RelayCommand(StopRecord);
            AddNewCameraCommand = new RelayCommand(AddNewCamera);
            IpCameraUrl = "http://109.7.231.170:8082/mjpg/video.mjpg";
            IpCameraUrls = new ObservableCollection<CameraScreen>();
            MonitoringImages = new ObservableCollection<MonitoringImage>();
            _fpsIndexer = 0;
        }

        public ObservableCollection<CameraScreen> IpCameraUrls
        {
            get { return _ipCameraUrls; }
            set { Set(ref _ipCameraUrls, value); }
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

        public ObservableCollection<MonitoringImage> MonitoringImages
        {
            get { return _monitoringImages; }
            set { Set(ref _monitoringImages, value); }
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

        public ICommand MakeGrayscaleCommand { get; private set; }

        public ICommand MakeOriginalCommand { get; private set; }

        public ICommand StopRecordVideoSourceCommand { get; private set; }

        public ICommand PreviousWindowCommand { get; private set; }

        public ICommand AddNewCameraCommand { get; private set; }

        public Action CloseAction { get; set; }

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
            _videoSource = new MJPEGStream("http://158.58.130.148:80/mjpg/video.mjpg");
            _videoSource.NewFrame += CaptureFace_Frame;
            _videoSource.Start();
        }

        private void StartCameras()
        {
            _videoSource = new MJPEGStream(IpCameraUrls.Last().ImageSourcePath);
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

        private void GetToPreviousWindow()
        {
            var menu = new MenuView();
            menu.Show();
            CloseAction();
        }

        public void AddNewCamera()
        {
            var newCameraView = new NewCameraView();
            newCameraView.ShowDialog();
            var url = ((NewCameraViewModel)newCameraView.DataContext).NewIpCameraUrl;
            IpCameraUrls.Add(new CameraScreen { ImageSourcePath = url });
            RaisePropertyChanged("IpCameraUrls");

            var indexer = 0;
            indexer++;
            var monitoringImage = new MonitoringImage(indexer, url);
            MonitoringImages.Add(monitoringImage);

            StartCameras();
        }

        private void CaptureFace_Frame(object sender, NewFrameEventArgs eventArgs)
        {
            var monitoringImage = MonitoringImages.FirstOrDefault(mi => mi.Source == ((MJPEGStream)sender).Source);
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
                Dispatcher.CurrentDispatcher.Invoke(() => monitoringImage.Image = bitmapImage);
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
