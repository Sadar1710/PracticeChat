using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeChat.Models
{
    public class GroupMeassage
    {
        [Key]
        public int Id { get; set; }
        public int GroupId { get; set; }
        public CreateGroup createGroup { get; set; }
        public string SenderId { get; set; }
        public string Message { get; set; }
        public DateTime When { get; set; }
        public GroupMeassage()
        {
            When = DateTime.Now;
        }
    }
}
