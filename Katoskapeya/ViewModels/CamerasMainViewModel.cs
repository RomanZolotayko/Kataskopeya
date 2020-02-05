using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Vision.Motion;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kataskopeya.Extensions;
using Kataskopeya.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
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
        private CascadeClassifier _haarCascade;
        private MotionDetector _motionDetector;
        private DispatcherTimer _timer;
        private DispatcherTimer _scanner;
        private Capture _capture;
        private Image<Bgr, byte> _currentFrame;
        private Image<Gray, byte> _result;
        private byte[] _monitoringImage;
        private RecognizerEngine _engine;

        public CamerasMainViewModel()
        {
            StartSourceCommand = new RelayCommand(StartCamera);
            StopSourceCommand = new RelayCommand(StopCamera);
            IpCameraUrl = "http://192.168.127.123:8080/video";
            _motionDetector = new MotionDetector(new TwoFramesDifferenceDetector(), new MotionBorderHighlighting());
            string recognizerPath = @"recognizer.YAML";
            _engine = new RecognizerEngine(recognizerPath);
            _engine.TrainRecognizer();

        }

        public byte[] MonitoringImage
        {
            get { return _monitoringImage; }
            set { Set(ref _monitoringImage, value); }
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
            _haarCascade = new CascadeClassifier(@"haarcascade_frontalface_alt_tree.xml");
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
                        var grayFrame = new Image<Bgr, byte>(bitmap);
                        var rectangles = _haarCascade.DetectMultiScale(grayFrame, 1.2, 1, System.Drawing.Size.Empty);

                        foreach (var rect in rectangles)
                        {
                            using (var graphics = Graphics.FromImage(bitmap))
                            {
                                using (var pen = new Pen(Color.Yellow, 10))
                                {
                                    graphics.DrawRectangle(pen, rect);
                                }
                            }
                            _result = grayFrame.Copy(rect).Convert<Gray, byte>().Resize(200, 200, Inter.Cubic);

                            int user = _engine.RecognizeUser(_result);

                            if (user == 2)
                            {
                                var test = "";
                            }
                            else if (user == 1)
                            {
                                var test = "";
                            }
                        }

                        //_motionDetector.ProcessFrame(bitmap);
                        bitmapImage = bitmap.ToBitmapImage();
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
