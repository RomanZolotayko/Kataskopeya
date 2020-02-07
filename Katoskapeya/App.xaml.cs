using Kataskopeya.EF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Kataskopeya
{
    public partial class App : Application
    {
        private ApplicationContext _context;

        public App()
        {
            _context = new ApplicationContext();
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!_context.Users.Any())
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
                    Name = "Master",
                    Password = "Qwe",
                    Age = 23,
                    Faces = myPhotos
                });

                _context.SaveChanges();
            }
        }
    }
}
