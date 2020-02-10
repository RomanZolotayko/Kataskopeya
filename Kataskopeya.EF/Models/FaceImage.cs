using System.Collections.Generic;

namespace Kataskopeya.EF.Models
{
    public class FaceImage : Base
    {
        public byte[] Face { get; set; }

        public ICollection<UserFaceImage> UserFaceImages { get; set; }
    }
}
