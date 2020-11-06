using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeChat.ViewModels
{
    public class OneToOneVM
    {
        public string FriendId { get; set; }
        public string FriendUserName { get; set; }
        public string PhotoPath { get; set; }      
        public string Message { get; set; }
        
    }
}
