using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Kataskopeya.Common.Constants;
using Kataskopeya.EF;
using System.Drawing;
using System.Drawing.Imaging;

namespace Kataskopeya.Services
{
    public class FaceScannerService
    {
        private readonly CascadeClassifier _cascadeClassifier;
        private readonly KataskopeyaContext _context;
        private int _nameTracker;
        private int _indexer;

        public FaceScannerService()
        {
            _cascadeClassifier = new CascadeClassifier(@"haarcascade_frontalface_alt_tree.xml");
            _context = new KataskopeyaContext();
        }

        public void Scan(Bitmap bitmap, string username)
        {
            var size = new Size(30, 30);

            var grayFrame = new Image<Bgr, byte>(bitmap);
            var rectangles = _cascadeClassifier.DetectMultiScale(grayFrame, 1.5, 1, size);
            _indexer++;

            foreach (var rect in rectangles)
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {

                    using (var pen = new Pen(Color.Red, 2))
                    {
                        graphics.DrawRectangle(pen, rect);
                    }


                    if (_indexer % 30 == 0)
                    {
                        var faceScreenShot = grayFrame.Copy(rect).Convert<Gray, byte>().Resize(100, 100, Inter.Cubic);
                        faceScreenShot.Bitmap.Save(FileSystemPaths.ScannerOutput + $"\\{username}\\" + $"/{_nameTracker++}_" + "faceimage.png", ImageFormat.Png);
                    }
                }
            }
        }
    }
}
