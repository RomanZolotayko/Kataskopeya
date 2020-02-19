using Kataskopeya.Common.Constants;
using Kataskopeya.EF;
using Kataskopeya.EF.Models;
using System.Collections.Generic;
using System.IO;

namespace Kataskopeya.Processors
{
    public class FaceImageProcessor
    {
        private KataskopeyaContext _context;

        public FaceImageProcessor()
        {
            _context = new KataskopeyaContext();
        }

        public void ProcessScannedImages(User user)
        {

            var directiory = new DirectoryInfo(FileSystemPaths.ScannerOutput + user.Name);
            var files = directiory.GetFiles("*.png");

            var photos = new List<byte[]>();

            foreach (var photo in files)
            {
                photos.Add(File.ReadAllBytes(FileSystemPaths.ScannerOutput + user.Name + "/" + photo.Name));
            }

            _context.Users.Add(user);

            var userImages = new List<UserFaceImage>();

            foreach (var image in photos)
            {
                var userImage = new UserFaceImage
                {
                    FaceImage = new FaceImage
                    {
                        Face = image
                    },
                    UserId = user.Id
                };

                userImages.Add(userImage);
            }

            _context.UserFaceImages.AddRange(userImages);

            _context.SaveChanges();
        }
    }
}
