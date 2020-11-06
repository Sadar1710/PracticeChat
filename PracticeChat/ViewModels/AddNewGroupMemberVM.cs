using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeChat.ViewModels
{
    public class AddNewGroupMemberVM
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PhotoPath { get; set; }
    }
}
