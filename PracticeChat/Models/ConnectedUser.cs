using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeChat.Models
{
    public class ConnectedUser
    {
        public static List<ConnectedUserInfo> connectedUserInfos { get; set; } = new List<ConnectedUserInfo>();
        //public static List<string> Ids = new List<string>();       
        //public static List<string> userName = new List<string>();        
    }
    public class ConnectedUserInfo
    {
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
    }
}
