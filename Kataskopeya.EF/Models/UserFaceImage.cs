namespace Kataskopeya.EF.Models
{
    public class UserFaceImage
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public int FaceImageId { get; set; }

        public FaceImage FaceImage { get; set; }
    }
}
