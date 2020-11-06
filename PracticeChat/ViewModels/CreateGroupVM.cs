using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeChat.ViewModels
{
    public class CreateGroupVM
    {        
        [Required]
        public string GroupName { get; set; }
        public List<string> UserId { get; set; }
    }
}
