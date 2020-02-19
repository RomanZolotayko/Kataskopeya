using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Kataskopeya.EF;
using Kataskopeya.Helpers;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Kataskopeya.Services
{
    public class FaceAnalyzerService
    {
        private readonly CascadeClassifier _cascadeClassifier;
        private readonly KataskopeyaContext _context;
        private RecognizerEngine _recognitionEngine;

        public FaceAnalyzerService()
        {
            _cascadeClassifier = new CascadeClassifier(@"haarcascade_frontalface_alt_tree.xml");
            _context = new KataskopeyaContext();
            _recognitionEngine = new RecognizerEngine(@"trainningData.YAML");
            //Task.Run(() => _recognitionEngine.TrainRecognizer()).Wait();
        }

        public string RecognizedUser { get; set; }

        public string Analyze(Bitmap bitmap, int fpsIndexer)
        {
            var size = new Size(30, 30);

            var grayFrame = new Image<Bgr, byte>(bitmap);
            var rectangles = _cascadeClassifier.DetectMultiScale(grayFrame, 1.5, 1, size);

            foreach (var rect in rectangles)
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {

                    using (var pen = new Pen(Color.Red, 2))
                    {
                        graphics.DrawRectangle(pen, rect);
                    }

                    if (fpsIndexer % 30 == 0)
                    {
                        var catchedFace = grayFrame.Copy(rect).Convert<Gray, byte>().Resize(100, 100, Inter.Cubic);
                        var label = _recognitionEngine.RecognizeUser(catchedFace);
                        var user = _context.Users.FirstOrDefault(x => x.Id == label);
                        RecognizedUser = user == null ? RecognizedUser = "Unknown" : RecognizedUser = user.Name;
                    }

                }
            }

            return RecognizedUser;
        }
    }
}
