using AForge.Video;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kataskopeya.Common.Constants;
using Kataskopeya.EF.Models;
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
        private IVideoSource _videoSource;
        private BitmapImage _image;
        private ObservableCollection<MonitoringImage> _monitoringImages;
        private ObservableCollection<CameraScreen> _ipCameraUrls;
        private readonly VideoRecordingService _videoRecordingService;
        private readonly CamerasService _camerasService;
        private int _displayWidth;
        private int _displayHeight;

        public CamerasMainViewModel()
        {
            DisplayHeight = DisplayData.DisplayHeight;
            DisplayWidth = DisplayData.DisplayWidth;
            _videoRecordingService = new VideoRecordingService();
            PreviousWindowCommand = new RelayCommand(GetToPreviousWindow);
            AddNewCameraCommand = new RelayCommand(AddNewCamera);
            IpCameraUrls = new ObservableCollection<CameraScreen>();
            MonitoringImages = new ObservableCollection<MonitoringImage>();
            _camerasService = new CamerasService();
            PrepareWindowToWork();
        }

        public ObservableCollection<CameraScreen> IpCameraUrls
        {
            get { return _ipCameraUrls; }
            set { Set(ref _ipCameraUrls, value); }
        }

        public int HalfWidth
        {
            get { return _displayWidth / 2; }
        }

        public int HalfHeight
        {
            get { return _displayHeight / 2; }
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

        public int DisplayWidth
        {
            get { return _displayWidth; }
            set { Set(ref _displayWidth, value); }
        }

        public int DisplayHeight
        {
            get { return _displayHeight; }
            set { Set(ref _displayHeight, value); }
        }

        public ICommand PreviousWindowCommand { get; private set; }

        public ICommand AddNewCameraCommand { get; private set; }

        public Action CloseAction { get; set; }

        private void StartLastAddedCamera()
        {
            _videoSource = new MJPEGStream(IpCameraUrls.Last().ImageSourcePath);
            var image = MonitoringImages.FirstOrDefault(mi => mi.Url == IpCameraUrls.Last().ImageSourcePath);
            image.VideoSource = _videoSource;
            _videoSource.NewFrame += CaptureVideo_Frame;
            _videoSource.Start();
        }

        private void StartAllCameras()
        {
            foreach (var camera in MonitoringImages)
            {
                camera.VideoRecordingService = new VideoRecordingService();
                var videoSource = new MJPEGStream(camera.Url);
                camera.VideoSource = videoSource;
                camera.VideoSource.NewFrame += CaptureVideo_Frame;
                camera.VideoSource.Start();
            }
        }

        private void GetToPreviousWindow()
        {
            var menu = new MenuView();
            menu.Show();

            foreach (var image in MonitoringImages)
            {
                image.VideoSource.Stop();
            }

            CloseAction();
        }

        public async void AddNewCamera()
        {
            var newCameraView = new NewCameraView();
            newCameraView.ShowDialog();
            var viewModel = ((NewCameraViewModel)newCameraView.DataContext);

            var url = viewModel.IpCameraUrl;
            var cameraName = viewModel.CameraName;

            if (url != null)
            {

                if (await _camerasService.IsCameraExists(url))
                {
                    MessageBox.Show("Url that you are trying to add is already exists.", "Exception", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                IpCameraUrls.Add(new CameraScreen { ImageSourcePath = url });
                RaisePropertyChanged("IpCameraUrls");

                var monitoringImage = new MonitoringImage(url, HalfWidth - 20, HalfHeight);
                monitoringImage.VideoRecordingService = new VideoRecordingService();
                monitoringImage.CameraName = cameraName;
                MonitoringImages.Add(monitoringImage);

                var camera = new Camera
                {
                    Url = url,
                    Name = cameraName
                };

                _camerasService.SaveNewCamera(camera);

                monitoringImage.CameraId = camera.Id.ToString();

                StartLastAddedCamera();
            }
        }

        private void CaptureVideo_Frame(object sender, NewFrameEventArgs eventArgs)
        {
            var monitoringImage = MonitoringImages.FirstOrDefault(mi => mi.Url == ((MJPEGStream)sender).Source);
            try
            {
                BitmapImage bitmapImage;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    bitmapImage = bitmap.ToBitmapImage();

                    if (monitoringImage.IsRecordSetupNeed)
                    {
                        monitoringImage.IsRecordSetupNeed = false;
                        monitoringImage.VideoRecordingService.SetUpRecordingEngine(bitmap.Width, bitmap.Height, monitoringImage.CameraName);
                    }

                    monitoringImage.VideoRecordingService.StartVideoRecording(bitmap);
                }

                bitmapImage.Freeze();
                Dispatcher.CurrentDispatcher.Invoke(() => monitoringImage.Image = bitmapImage);

            }
            catch (Exception ex)
            {
            }
        }

        private async void PrepareWindowToWork()
        {
            var cameras = await _camerasService.GetCameras();

            MonitoringImages.Clear();

            foreach (var camera in cameras)
            {
                var monitoringImage = new MonitoringImage
                {
                    CameraName = camera.Name,
                    CameraId = camera.Id.ToString(),
                    Url = camera.Url,
                    GridWidth = HalfWidth - 20,
                    GridHeight = HalfHeight,
                    IsRecordSetupNeed = true
                };

                MonitoringImages.Add(monitoringImage);
            }

            RaisePropertyChanged("MonitoringImages");

            StartAllCameras();
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
