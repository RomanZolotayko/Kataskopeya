using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kataskopeya.EF.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public byte[] FaceImage { get; set; }
    }
}
