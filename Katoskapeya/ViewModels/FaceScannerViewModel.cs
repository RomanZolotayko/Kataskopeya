using AForge.Video;
using AForge.Video.DirectShow;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kataskopeya.Common.Constants;
using Kataskopeya.EF.Models;
using Kataskopeya.Extensions;
using Kataskopeya.Helpers;
using Kataskopeya.Models;
using Kataskopeya.Processors;
using Kataskopeya.Services;
using Kataskopeya.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Kataskopeya.ViewModels
{
    public class FaceScannerViewModel : ObservableObject, IDisposable
    {
        private BitmapImage _image;
        private IVideoSource _videoSource;
        private readonly FaceScannerService _faceScannerService;
        private List<Station> _stations;
        private string _userMessage;
        private Station _currentStation;
        private bool _isScanButtonEnabled;
        private Visibility _panelsVisability;
        private Visibility _spinnerVisability;
        private Visibility _thumbUpVisability;
        private int _age;
        private FaceImageProcessor _faceImageProcessor;
        private RecognizerEngine _recognizerEngine;

        public FaceScannerViewModel()
        {
            PanelsVisability = Visibility.Visible;
            SpinnerVisability = Visibility.Hidden;
            ThumbUpVisability = Visibility.Hidden;
            _recognizerEngine = new RecognizerEngine(@"trainningData.YAML");
            _faceImageProcessor = new FaceImageProcessor();
            _faceScannerService = new FaceScannerService();
            PreviousWindowCommand = new RelayCommand(GetToPreviousWindow);
            StartScanCommand = new RelayCommand(StartScan);
            CreateUserStations();
            _currentStation = _stations.First();
            UserMessage = _currentStation.Message;
            Directory.CreateDirectory(FileSystemPaths.ScannerOutput);
            IsScanButtonEnabled = true;
        }

        public BitmapImage Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }

        public string UserMessage
        {
            get { return _userMessage; }
            set { Set(ref _userMessage, value); }
        }

        public int Age
        {
            get { return _age; }
            set { Set(ref _age, value); }
        }

        public Visibility SpinnerVisability
        {
            get { return _spinnerVisability; }
            set { Set(ref _spinnerVisability, value); }
        }

        public Visibility ThumbUpVisability
        {
            get { return _thumbUpVisability; }
            set { Set(ref _thumbUpVisability, value); }
        }

        public Visibility PanelsVisability
        {
            get { return _panelsVisability; }
            set { Set(ref _panelsVisability, value); }
        }

        public bool IsScanButtonEnabled
        {
            get { return _isScanButtonEnabled; }
            set { Set(ref _isScanButtonEnabled, value); }
        }

        public string Username { get; set; }

        public FilterInfo CapturedDevice { get; set; }

        public ICommand PreviousWindowCommand { get; set; }

        public ICommand StartScanCommand { get; set; }

        public Action CloseAction { get; set; }

        private void StartCamera()
        {
            Directory.CreateDirectory(FileSystemPaths.ScannerOutput);
            Directory.CreateDirectory(FileSystemPaths.ScannerOutput + "/" + Username);

            var capturedDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            foreach (FilterInfo device in capturedDevices)
            {
                CapturedDevice = device;
            }

            IsScanButtonEnabled = false;
            RaisePropertyChanged("IsScanButtonEnabled");
            _videoSource = new VideoCaptureDevice(CapturedDevice.MonikerString);
            _videoSource.NewFrame += ProcessVideo_Frame;
            _videoSource.Start();
        }

        public void GetToPreviousWindow()
        {
            _videoSource?.Stop();
            var scanner = new ScanView();
            scanner.Show();
            CloseAction();
        }

        private void StopFaceScanning()
        {
            _videoSource.Stop();
            _stations.Remove(_stations.First());
            SetNextStation(_stations.First());

            IsScanButtonEnabled = true;

            if (_currentStation.IsLastStation)
            {
                PanelsVisability = Visibility.Hidden;
                SpinnerVisability = Visibility.Visible;
                RaisePropertyChanged("PanelsVisability");
                RaisePropertyChanged("SpinnerVisability");

                IsScanButtonEnabled = false;
                ProcessResults();
                _recognizerEngine.TrainRecognizerByUser(Username);

                SpinnerVisability = Visibility.Hidden;
                ThumbUpVisability = Visibility.Visible;
                RaisePropertyChanged("SpinnerVisability");
                RaisePropertyChanged("ThumbUpVisability");
            }

            RaisePropertyChanged("IsScanButtonEnabled");
        }

        private void StartScan()
        {
            StartCamera();
            Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(x => StopFaceScanning());
        }

        private void ProcessVideo_Frame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                BitmapImage bitmapImage;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    _faceScannerService.Scan(bitmap, Username);
                    bitmapImage = bitmap.ToBitmapImage();
                }

                bitmapImage.Freeze();
                Dispatcher.CurrentDispatcher.Invoke(() => Image = bitmapImage);

            }
            catch (Exception ex)
            {

            }
        }

        private void CreateUserStations()
        {
            var firstStation = new Station("After you pressed button, look straight in the camera, and slowly back from it.(Press the button)");
            var secondStation = new Station("Now, look in the camera and slowly turn your head right.(Press the button)");
            var thirdStation = new Station("Now, look in the camera and slowly turn your head left.(Press the button)");
            var fourthStation = new Station("Now, look in the camera and slowly turn your head up.(Press the button)");
            var fifthStation = new Station("Now, look in the camera and slowly turn your head down.(Press the button)");
            var sixthStation = new Station("We are finished here, thank you!", true);

            var stations = new List<Station>
            {
                firstStation,
                secondStation,
                thirdStation,
                fourthStation,
                fifthStation,
                sixthStation
            };

            _stations = new List<Station>();
            _stations.AddRange(stations);
        }

        private void SetNextStation(Station station)
        {
            _currentStation = station;
            UserMessage = station.Message;

            RaisePropertyChanged("UserMessage");
        }

        private void ProcessResults()
        {
            var user = new User
            {
                Age = Age,
                Name = Username
            };

            _faceImageProcessor.ProcessScannedImages(user);
        }

        public void Dispose()
        {
            CloseAction();
        }
    }
}
