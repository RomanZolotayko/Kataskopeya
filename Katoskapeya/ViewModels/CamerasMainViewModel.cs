using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Vision.Motion;
using Emgu.CV;
using Emgu.CV.Structure;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kataskopeya.Extensions;
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
        private string _ipCameraUrl;
        private IVideoSource _videoSource;
        private bool _isIpCameraSource;
        private bool _original;
        private bool _grayscale;
        private bool _thresholded;
        private int _threshold;
        private CascadeClassifier _cascadeClassifier;
        private MotionDetector _motionDetector;

        public CamerasMainViewModel()
        {
            StartSourceCommand = new RelayCommand(StartCamera);
            StopSourceCommand = new RelayCommand(StopCamera);
            IpCameraUrl = "http://192.168.127.123:8080/video";
            _cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
            _motionDetector = new MotionDetector(new TwoFramesDifferenceDetector(), new MotionBorderHighlighting());
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

        public bool Thresholded
        {
            get { return _thresholded; }
            set { Set(ref _thresholded, value); }
        }

        public int Threshold
        {
            get { return _threshold; }
            set { Set(ref _threshold, value); }
        }

        public bool IsIpCameraSource
        {
            get { return _isIpCameraSource; }
            set { Set(ref _isIpCameraSource, value); }
        }

        public string IpCameraUrl
        {
            get { return _ipCameraUrl; }
            set { Set(ref _ipCameraUrl, value); }
        }

        public ICommand StartSourceCommand { get; private set; }

        public ICommand StopSourceCommand { get; private set; }

        private void StartCamera()
        {
            _videoSource = new MJPEGStream(IpCameraUrl);
            _videoSource.NewFrame += video_NewFrame;
            _videoSource.Start();
        }

        private void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= video_NewFrame;
            }
            Image = null;
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
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
                    else if (Thresholded)
                    {
                        using (var grayscaledBitmap = Grayscale.CommonAlgorithms.BT709.Apply(bitmap))
                        using (var thresholdedBitmap = new Threshold(Threshold).Apply(grayscaledBitmap))
                        {
                            bitmapImage = thresholdedBitmap.ToBitmapImage();
                        }
                    }
                    else
                    {

                        //_motionDetector.ProcessFrame(bitmap);
                        //bitmapImage = bitmap.ToBitmapImage();
                        //Image<Bgr, Byte> currentFrame = new Image<Bgr, byte>(bitmap);

                        //if (currentFrame != null)
                        //{
                        //    Image<Gray, Byte> grayFrame = = new Image<;

                        //    var detectedFaces = _cascadeClassifier.DetectMultiScale(grayFrame, 1.2, 10, System.Drawing.Size.Empty);

                        //    foreach (var face in detectedFaces)
                        //    {
                        //        currentFrame.Draw(face, new Bgr(System.Drawing.Color.Red), 3);
                        //    }

                        //}

                        //var cameraCapture = new Capture("http://192.168.127.123:8080/video");
                        //Mat frame = cameraCapture.QueryFrame();

                        //var grayImage = new Image<Bgr, byte>(frame?.Bitmap);
                        //var rectangles = _cascadeClassifier.DetectMultiScale(grayImage, 1.2, 1);
                        //foreach (var rect in rectangles)
                        //{
                        //    using (var graphics = Graphics.FromImage(frame?.Bitmap))
                        //    {
                        //        using (var pen = new Pen(Color.Red, 1))
                        //        {
                        //            graphics.DrawRectangle(pen, rect);
                        //        }
                        //    }
                        //}

                        //_motionDetector.ProcessFrame(bitmap);
                        //bitmapImage = frame?.Bitmap.ToBitmapImage();
                    }
                }

                bitmapImage.Freeze(); // avoid cross thread operations and prevents leaks
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
