using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeChat.Models
{
    public class CreateGroup
    {
        [Key]
        public int Id { get; set; }
        public int GroupId { get; set; }        
        public GroupInfo GroupInfo { get; set; }
        public string UserId { get; set; }        
    }
}
