using Microsoft.AspNetCore.SignalR;
using PracticeChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{    
    public class ChatHub : Hub
    {
        //static HashSet<string> CurrentConnections = new HashSet<string>();
        public Task SendMessageToAll(string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", message);
        }
        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.SendAsync("ReceiveMz", Context.User.Identity.Name, message);
        }
        public Task SendMessageToUser(string connectionId,string receiverId, string message)
        {
            return Clients.Client(connectionId).SendAsync("ReceiveMessage", Context.User.Identity.Name,receiverId, message);
        }       
        public override Task OnConnectedAsync()
        {            
            ConnectedUserInfo connectedUserInfo = new ConnectedUserInfo();
            connectedUserInfo.UserName = Context.User.Identity.Name;
            connectedUserInfo.ConnectionId = Context.ConnectionId;
            ConnectedUser.connectedUserInfos.Add(connectedUserInfo);           
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception ex)
        {
            var a= ConnectedUser.connectedUserInfos;
            var b = a.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();
            ConnectedUser.connectedUserInfos.Remove(b);           
            return base.OnDisconnectedAsync(ex);
        }       
        public Task JoinGroup(string GroupName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, GroupName);
        }
        //public Task JoinGroup(string connectionId, string GroupName)
        //{
        //    return Groups.AddToGroupAsync(connectionId, GroupName);
        //}
        public Task SendMessageToGroups(string group,string Sender,string message)
        {
            return Clients.Groups(group).SendAsync("ReceiveMessage",group, Sender, message);        
        }
    }
}

