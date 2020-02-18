using Accord.Video.FFMPEG;
using Kataskopeya.Extensions;
using Microsoft.Win32;
using System;
using System.Drawing;

namespace Kataskopeya.Services
{
    public class VideoRecordingService
    {
        private readonly VideoFileWriter _writer;
        private DateTime? _firstFrameTime;
        private int _videoDurationTracker;
        private int _frapsPerMinute = 1500;

        public VideoRecordingService()
        {
            _writer = new VideoFileWriter();
            _videoDurationTracker = 0;
        }

        public double Width { get; set; }

        public double Height { get; set; }

        public string FilePath { get; set; }

        public string CameraName { get; set; }

        public void SetUpRecordingEngine(double width, double height, string cameraName)
        {
            var dialog = new SaveFileDialog();

            Width = width;
            Height = height;
            CameraName = cameraName;

            var proceededCameraName = cameraName.GetVideoFileName();

            dialog.FileName = AppDomain.CurrentDomain.BaseDirectory + "SurvellianceMaterials\\" + $"{proceededCameraName}" + ".avi";
            FilePath = dialog.FileName;
            dialog.AddExtension = true;
            _writer.Open(dialog.FileName, (int)Math.Round(Width, 0), (int)Math.Round(Height, 0));
        }

        public void StartVideoRecording(Bitmap frame)
        {
            _videoDurationTracker++;

            var durationOfRecordedVideoChunk = SettingsService.GetApplicationSettings().DurationOfRecordedVideoChunk;
            var chunkTimeSize = _frapsPerMinute * durationOfRecordedVideoChunk;

            if (_videoDurationTracker % chunkTimeSize == 0)
            {
                var filePath = AppDomain
                    .CurrentDomain
                    .BaseDirectory + "SurvellianceMaterials\\" + $"{CameraName.GetVideoFileName()}" + ".avi";
                _writer.Close();
                _writer.Open(filePath, (int)Width, (int)Height);
                _videoDurationTracker = 0;
            }

            if (_firstFrameTime != null)
            {
                _writer.WriteVideoFrame(frame);
            }
            else
            {
                _writer.WriteVideoFrame(frame);
                _firstFrameTime = DateTime.Now;
            }
        }

        public void StopVideoRecording()
        {
            _writer.Close();
            _writer.Dispose();
        }
    }
}
