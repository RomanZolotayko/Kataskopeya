using Kataskopeya.EF;
using Kataskopeya.EF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kataskopeya.Helpers
{
    public class DatabaseHelper
    {
        private ApplicationContext _context;

        public DatabaseHelper()
        {
            _context = new ApplicationContext();
        }

        public void GenerateTrainingSeeds()
        {
            var directiory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "dataSet");
            FileInfo[] files = directiory.GetFiles("*.jpg");

            var obamaPictures = files.Where(x => x.Name.Contains("obama"));

            var obamaPicturesConvertedToByte = new List<byte[]>();

            foreach (var photo in obamaPictures)
            {
                obamaPicturesConvertedToByte.Add(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + $"dataSet/{photo.Name}"));
            }

            _context.Users.Add(new User
            {
                Name = "President Obama",
                Age = 40,
                Faces = obamaPicturesConvertedToByte
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
        }
    }
}
