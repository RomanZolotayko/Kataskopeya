using Kataskopeya.EF.Models;
using System;
using System.Data.Entity.Migrations;
using System.IO;

namespace Kataskopeya.EF.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Kataskopeya.EF.ApplicationContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Kataskopeya.EF.ApplicationContext context)
        {
            var imageDataFace = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "FaceImages/face.jpg");
            var imageDataMy = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "FaceImages/myface.jpg");
            var imageDataDucalis = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "FaceImages/ducalis.jpg");
            var imageDataChinese = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "FaceImages/chinese.jpg");

            var face = new User
            {
                Age = 23,
                Name = "Face",
                FaceImage = imageDataFace
            };

            var me = new User
            {
                Age = 23,
                Name = "Roman",
                FaceImage = imageDataMy
            };

            var ducalis = new User
            {
                Age = 23,
                Name = "Ducalis",
                FaceImage = imageDataDucalis
            };

            var chinese = new User
            {
                Age = 23,
                Name = "Chinese",
                FaceImage = imageDataChinese
            };

            context.Users.Add(face);
            context.Users.Add(me);
            context.Users.Add(ducalis);
            context.Users.Add(chinese);
            context.SaveChanges();
        }
    }
}
