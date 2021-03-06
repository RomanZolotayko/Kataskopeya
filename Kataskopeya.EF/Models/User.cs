﻿using System.Collections.Generic;

namespace Kataskopeya.EF.Models
{
    public class User : Base
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public int Age { get; set; }

        public ICollection<UserFaceImage> UserFaceImages { get; set; }
    }
}
