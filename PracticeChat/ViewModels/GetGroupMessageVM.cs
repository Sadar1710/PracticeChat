using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeChat.ViewModels
{
    public class GetGroupMessageVM
    {
        public int GroupId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string DateTime { get; set; }
    }
}
