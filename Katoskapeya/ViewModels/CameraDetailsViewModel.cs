using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kataskopeya.Common.Constants;
using Kataskopeya.Common.Enums;
using Kataskopeya.Extensions;
using Kataskopeya.Services;
using System;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Kataskopeya.ViewModels
{
    public class CameraDetailsViewModel : ObservableObject, IDisposable
    {
        private BitmapImage _image;
        private IVideoSource _videoSource;
        private CameraModes _cameraMode;
        private readonly VideoRecordingService _videoRecordingService;
        private readonly MotionDetectionService _motionDetectionService;
        private readonly FaceAnalyzerService _faceAnalyzerService;
        private bool _isFaceCaptureEnabled;
        private int _fpsIndexer;
        private string _analyzeResult;


        public CameraDetailsViewModel(string cameraUrl)
        {
            PreviousWindowCommand = new RelayCommand(GetToPreviousWindow);
            EnableFaceCaptureCommand = new RelayCommand(EnableFaceCaptureMode);
            EnableGrayscaleModeCommand = new RelayCommand(EnableGrayscaleMode);
            EnableOriginalModeCommand = new RelayCommand(EnableOriginalMode);
            EnableMotionCaptureModeCommand = new RelayCommand(EnableMotionCaptureMode);

            _videoRecordingService = new VideoRecordingService();
            _motionDetectionService = new MotionDetectionService();
            _faceAnalyzerService = new FaceAnalyzerService();

            DisplayWidth = DisplayData.DisplayWidth;
            DisplayHeight = DisplayData.DisplayHeight;

            _cameraMode = CameraModes.Original;

            if (!string.IsNullOrEmpty(cameraUrl))
            {
                CameraUrl = cameraUrl;
                StartCamera();
            }

        }

        public CameraDetailsViewModel()
        {

        }

        public BitmapImage Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }

        public string AnalyzeResult
        {
            get { return _analyzeResult; }
            set { Set(ref _analyzeResult, value); }
        }

        public bool IsFaceCaptureEnabled
        {
            get { return _isFaceCaptureEnabled; }
            set { Set(ref _isFaceCaptureEnabled, value); }
        }

        public int DisplayWidth { get; set; }

        public int DisplayHeight { get; set; }

        public string CameraUrl { get; set; }

        public Action CloseAction { get; set; }

        public ICommand PreviousWindowCommand { get; set; }

        public ICommand EnableFaceCaptureCommand { get; set; }

        public ICommand EnableGrayscaleModeCommand { get; set; }

        public ICommand EnableOriginalModeCommand { get; set; }

        public ICommand EnableMotionCaptureModeCommand { get; set; }

        public FilterInfo CapturedDevice { get; set; }

        private void StartCamera()
        {
            //var capturedDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            //foreach (FilterInfo device in capturedDevices)
            //{
            //    CapturedDevice = device;
            //}

            //_videoSource = new VideoCaptureDevice(CapturedDevice.MonikerString);
            _videoSource = new MJPEGStream(CameraUrl);
            _videoSource.NewFrame += ProcessVideo_Frame;
            _videoSource.Start();
        }

        public void GetToPreviousWindow()
        {
            _videoSource.Stop();
            CloseAction();
        }

        public void EnableFaceCaptureMode()
        {
            _cameraMode = CameraModes.FaceCapture;
        }

        public void EnableGrayscaleMode()
        {
            _cameraMode = CameraModes.Grayscale;
        }

        public void EnableOriginalMode()
        {
            _cameraMode = CameraModes.Original;
        }

        public void EnableMotionCaptureMode()
        {
            _cameraMode = CameraModes.MotionCapture;
        }

        private void ProcessVideo_Frame(object sender, NewFrameEventArgs eventArgs)
        {
            _fpsIndexer++;

            try
            {
                BitmapImage bitmapImage;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    switch (_cameraMode)
                    {
                        case CameraModes.FaceCapture:
                            AnalyzeResult = _faceAnalyzerService.Analyze(bitmap, _fpsIndexer);
                            break;

                        case CameraModes.MotionCapture:
                            _motionDetectionService.Detect(bitmap);
                            break;

                        case CameraModes.Grayscale:
                            Grayscale.CommonAlgorithms.BT709.Apply(bitmap);
                            break;

                        case CameraModes.Original:
                            break;
                    }

                    bitmapImage = bitmap.ToBitmapImage();
                }

                bitmapImage.Freeze();
                Dispatcher.CurrentDispatcher.Invoke(() => Image = bitmapImage);
            }
            catch (Exception ex)
            {

            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
