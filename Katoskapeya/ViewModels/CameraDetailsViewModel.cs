using AForge.Video;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kataskopeya.Common.Constants;
using Kataskopeya.Extensions;
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
        private bool _isFaceCaptureEnabled;

        public CameraDetailsViewModel(string cameraUrl)
        {
            PreviousWindowCommand = new RelayCommand(GetToPreviousWindow);
            DisplayWidth = DisplayData.DisplayWidth;
            DisplayHeight = DisplayData.DisplayHeight;

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

        private void StartCamera()
        {
            _videoSource = new MJPEGStream(CameraUrl);
            _videoSource.NewFrame += ProcessVideo_Frame;
            _videoSource.Start();
        }

        public void GetToPreviousWindow()
        {
            CloseAction();
        }

        public void EnableFaceCapture()
        {
            if (IsFaceCaptureEnabled)
            {
                IsFaceCaptureEnabled = false;
            }
            else
            {
                IsFaceCaptureEnabled = true;
            }
        }

        private void ProcessVideo_Frame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                BitmapImage bitmapImage;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    //if (Grayscaled)
                    //{
                    //    using (var grayscaledBitmap = Grayscale.CommonAlgorithms.BT709.Apply(bitmap))
                    //    {

                    //        bitmapImage = grayscaledBitmap.ToBitmapImage();
                    //    }
                    //}
                    //else
                    //{
                    //    //_motionDetectionService.Detect(bitmap);
                    //    //_faceAnalyzerService.Analyze(bitmap, _fpsIndexer);
                    //    //_videoRecordingService.StartVideoRecording(bitmap);
                    //    bitmapImage = bitmap.ToBitmapImage();
                    //    _fpsIndexer++;
                    //}
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
