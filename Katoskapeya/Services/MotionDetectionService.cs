using AForge.Vision.Motion;
using System.Drawing;

namespace Kataskopeya.Services
{
    public class MotionDetectionService
    {
        private MotionDetector _motionDetector;

        public MotionDetectionService()
        {
            _motionDetector = new MotionDetector(new TwoFramesDifferenceDetector(), new MotionBorderHighlighting());
        }

        public void Detect(Bitmap bitmap)
        {
            _motionDetector.ProcessFrame(bitmap);
        }
    }
}
