using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Kataskopeya.EF;
using System;
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
        private ApplicationContext _context;

        public RecognizerEngine(string recognizerFilePath)
        {
            recognizerPath = recognizerFilePath;
            faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100);
            //recognizer = new EigenFaceRecognizer(80, double.PositiveInfinity);
            //recognizer = new FisherFaceRecognizer(0, 3500);//4000

            _context = new ApplicationContext();
        }

        public async void TrainRecognizer()
        {
            var directiory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "photosDataset");
            FileInfo[] files = directiory.GetFiles("*.png");

            var myFiles = files.Where(x => x.Name.Contains("myPhoto"));

            var myPhotos = new List<byte[]>();

            foreach (var photo in myFiles)
            {
                myPhotos.Add(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + $"photosDataset/{photo.Name}"));
            }

            _context.Users.Add(new EF.Models.User
            {
                Name = "Roman the Master",
                Age = 23,
                Faces = myPhotos
            });

            //var trumpFiles = files.Where(x => x.Name.Contains("User.2"));

            //var trumpPhotos = new List<byte[]>();

            //foreach (var photo in trumpFiles)
            //{
            //    trumpPhotos.Add(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + $"dataSet/{photo.Name}"));
            //}

            //_context.Users.Add(new EF.Models.User
            //{
            //    Name = "Trump",
            //    Age = 40,
            //    Faces = trumpPhotos
            //});

            //var mePhotos = new List<byte[]>();

            //var meFiles = files.Where(x => x.Name.Contains("me"));

            //foreach (var photo in meFiles)
            //{
            //    mePhotos.Add(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + $"dataSet/{photo.Name}"));
            //}

            //_context.Users.Add(new EF.Models.User
            //{
            //    Name = "Roman",
            //    Age = 40,
            //    Faces = mePhotos
            //});

            _context.SaveChanges();

            var users = await _context.Users.ToListAsync();
            if (users != null)
            {
                var faceImages = new List<Image<Gray, byte>>();
                var faceLabels = new List<int>();

                foreach (var user in users)
                {
                    Stream stream = new MemoryStream();
                    foreach (var face in user.Faces)
                    {
                        stream.Write(face, 0, face.Length);
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
