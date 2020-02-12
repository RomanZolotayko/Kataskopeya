using Kataskopeya.Common.Constants;
using Kataskopeya.EF;
using Kataskopeya.EF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kataskopeya.Helpers
{
    public static class DbSeedHelper
    {
        public static void SeedDatabase(KataskopeyaContext context)
        {
            if (!context.Users.Any())
            {
                var directiory = new DirectoryInfo(FileSystemPaths.DebugFolder + "photosDataset");
                var files = directiory.GetFiles("*.png");

                var myFiles = files.Where(x => x.Name.Contains("myPhoto"));

                var myPhotos = new List<byte[]>();

                foreach (var photo in myFiles)
                {
                    myPhotos.Add(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + $"photosDataset/{photo.Name}"));
                }

                var user = new User
                {
                    Name = "Master",
                    Password = "Qwe",
                    Age = 23,
                };

                context.Users.Add(user);

                foreach (var image in myPhotos)
                {
                    var userFaceImage = new UserFaceImage
                    {
                        FaceImage = new FaceImage
                        {
                            Face = image
                        },
                        UserId = user.Id
                    };

                    context.UserFaceImages.Add(userFaceImage);
                }

                context.SaveChanges();
            }
        }
    }
}
