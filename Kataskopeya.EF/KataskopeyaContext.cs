using Kataskopeya.EF.Models;
using System.Data.Entity;

namespace Kataskopeya.EF
{
    public class KataskopeyaContext : DbContext
    {
        public KataskopeyaContext() : base("KataskopeyaDB")
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<FaceImage> FaceImages { get; set; }

        public DbSet<UserFaceImage> UserFaceImages { get; set; }

        public DbSet<Camera> Cameras { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserFaceImage>().HasKey(ufi => new { ufi.UserId, ufi.FaceImageId });
        }
    }
}
