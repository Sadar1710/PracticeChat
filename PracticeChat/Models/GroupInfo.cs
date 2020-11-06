using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeChat.Models
{
    public class GroupInfo
    {
        [Key]
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public DateTime When { get; set; }
        public GroupInfo()
        {
            When = DateTime.Now;
        }
    }
}
