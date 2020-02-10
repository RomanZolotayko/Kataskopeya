using Accord.Video.FFMPEG;
using Microsoft.Win32;
using System;
using System.Drawing;

namespace Kataskopeya.Services
{
    public class VideoRecordingService
    {
        private readonly VideoFileWriter _writer;
        private DateTime? _firstFrameTime;

        public VideoRecordingService()
        {
            _writer = new VideoFileWriter();
        }

        public void SetUpRecordingEngine(double width, double height)
        {
            var dialog = new SaveFileDialog();

            dialog.FileName = "D:\\video\\video.avi";
            dialog.FileName = AppDomain.CurrentDomain.BaseDirectory + "SurvellianceMaterials\\" + DateTime.Now.ToString("MM/dd/yyyy").Replace(".", "_") + ".avi";
            dialog.AddExtension = true;
            _writer.Open(dialog.FileName, (int)Math.Round(width, 0), (int)Math.Round(height, 0));
        }

        public void StartVideoRecording(Bitmap frame)
        {
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
