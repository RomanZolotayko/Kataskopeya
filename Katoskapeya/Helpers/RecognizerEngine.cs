using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Kataskopeya.EF;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Kataskopeya.Helpers
{
    public class RecognizerEngine
    {
        private FaceRecognizer faceRecognizer;
        private string recognizerPath;
        private  ApplicationContext _context;

        public RecognizerEngine(string recognizerFilePath)
        {
            recognizerPath = recognizerFilePath;
            faceRecognizer = new FisherFaceRecognizer(0,3500);
            _context = new ApplicationContext();
        }

        public bool TrainRecognizer()
        {
            var allUsers = _context.Users.ToList();
            if (allUsers != null)
            {
                var faceImages = new List<Image<Gray, byte>>();
                var faceLabels = new List<int>();
                for (int i = 0; i < allUsers.Count; i++)
                {
                    Stream stream = new MemoryStream();
                    stream.Write(allUsers[i].FaceImage, 0, allUsers[i].FaceImage.Length);
                    var faceImage = new Image<Gray, byte>(new Bitmap(stream));
                    faceImages.Add(faceImage.Resize(100, 100, Inter.Cubic));
                    faceLabels.Add(allUsers[i].Id);
                }
                faceRecognizer.Train(faceImages.ToArray(), faceLabels.ToArray());
                faceRecognizer.Save(recognizerPath);
            }

            return true;
        }

        public int RecognizeUser(Image<Gray, byte> userImage)
        {
            faceRecognizer.Load(recognizerPath);
            var res = faceRecognizer.Predict(userImage.Resize(100, 100, Inter.Cubic));
            return res.Label;
        }
    }
}
