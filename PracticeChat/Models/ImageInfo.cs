using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeChat.Models
{
    public class ImageInfo
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string PhotoPath { get; set; }
    }
}
