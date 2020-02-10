using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Kataskopeya.EF;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Kataskopeya.Helpers
{
    public class RecognizerEngine
    {
        private FaceRecognizer faceRecognizer;
        private string recognizerPath;
        private KataskopeyaContext _context;

        public RecognizerEngine(string recognizerFilePath)
        {
            recognizerPath = recognizerFilePath;
            faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100);
            //recognizer = new EigenFaceRecognizer(80, double.PositiveInfinity);
            //recognizer = new FisherFaceRecognizer(0, 3500);//4000

            _context = new KataskopeyaContext();
        }

        public async void TrainRecognizer()
        {
            var users = await _context.Users.ToListAsync();
            if (users != null)
            {
                var faceImages = new List<Image<Gray, byte>>();
                var faceLabels = new List<int>();

                var dbUserPhotos = await _context.UserFaceImages.Include(ufi => ufi.FaceImage).ToListAsync();

                foreach (var user in users)
                {
                    var userPhotos = dbUserPhotos.Where(ufi => ufi.UserId == user.Id).Select(ufi => ufi.FaceImage);

                    Stream stream = new MemoryStream();
                    foreach (var item in userPhotos)
                    {
                        stream.Write(item.Face, 0, item.Face.Length);
                        var faceImage = new Image<Gray, byte>(new Bitmap(stream));
                        faceImages.Add(faceImage.Resize(100, 100, Inter.Cubic));
                        faceLabels.Add(user.Id);
                    }
                }

                faceRecognizer.Train(faceImages.ToArray(), faceLabels.ToArray());
                faceRecognizer.Save(recognizerPath);
            }
        }

        public int RecognizeUser(Image<Gray, byte> userImage)
        {
            faceRecognizer.Load(recognizerPath);
            var res = faceRecognizer.Predict(userImage.Convert<Gray, byte>().Resize(100, 100, Inter.Cubic));
            return res.Label;
        }
    }
}
