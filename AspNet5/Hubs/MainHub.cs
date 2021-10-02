using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet5.Hubs
{
    public class HubUser
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
    }

    public class HubUsers
    {
        private ConcurrentDictionary<string, HubUser> users { get; set; } = new ConcurrentDictionary<string, HubUser>();

        public void AddUser(HubCallerContext context)
        {
            HubUser user = new HubUser() { ConnectionId = context.ConnectionId, UserName = context.UserIdentifier };
            users.AddOrUpdate(user.UserName, user, (key, value) => user);
        }

        public void RemoveUser(HubCallerContext context)
        {
            HubUser user;
            users.TryRemove(context.UserIdentifier, out user);
        }

        public IEnumerable<HubUser> GetAllUsersExcept(string username)
        {
            return users.Values.Where(item => item.UserName != username);
        }

        public HubUser GetUserInfo(string username)
        {
            HubUser user;
            users.TryGetValue(username, out user);
            return user;
        }

        public List<string> AllUsers()
        {
            return users.Select(u => u.Value.UserName).ToList();
        }
    }


   // [Authorize]
    public class MainHub : Hub
    {
        private static HubUsers users = new HubUsers();

        public override async Task OnConnectedAsync()
        {
            users.AddUser(Context);
            await Groups.AddToGroupAsync(Context.ConnectionId, "MainHubUsers");
            await Clients.User(Context.UserIdentifier).SendAsync("UserList", JsonConvert.SerializeObject(users.AllUsers()));
            await Clients.Others.SendAsync("UserConnected", Context.UserIdentifier);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            users.RemoveUser(Context);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "MainHubUsers");
            await Clients.All.SendAsync("UserDisconnected", Context.UserIdentifier);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task BroadcastMessage(string message)
        {
            string Sender = $"{Context.User.Identity.Name}({Context.UserIdentifier})";
            await Clients.All.SendAsync("ReceiveMessage", Sender, message);
        }

        public async Task SendDirectMessage(string message, string targetUserName)
        {
            var userInfoSender = users.GetUserInfo(Context.User.Identity.Name);
            var userInfoReciever = users.GetUserInfo(targetUserName);
            await Clients.Client(userInfoReciever.ConnectionId).SendAsync("SendDM", message, userInfoSender);
        }

        private static Dictionary<string, string> connectionsNgroup = new Dictionary<string, string>();

        public async Task JoinGroup(string group)
        {
            if (connectionsNgroup.ContainsKey(Context.ConnectionId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connectionsNgroup[Context.ConnectionId]);
                connectionsNgroup.Remove(Context.ConnectionId);
            }
            connectionsNgroup.Add(Context.ConnectionId, group);
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }


        //ClaimsPrincipal userPrincipal = Context.User;

        //string loginUserIdentifier = Context.UserIdentifier;

        //string connectionId = Context.ConnectionId;


        //public override async Task OnConnectedAsync()
        //{
        //    string user = Context.UserIdentifier;


        //    await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
        //    await base.OnDisconnectedAsync(exception);
        //}

        //public Task SendPrivateMessage(string user, string message)
        //{
        //    return Clients.User(user).SendAsync("ReceiveMessage", message);
        //}

        //public async Task AddToGroup(string groupName)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        //    await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        //}

        //public async Task RemoveFromGroup(string groupName)
        //{
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        //    await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        //}

        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}

        //public Task SendMessageToCaller(string message)
        //{
        //    return Clients.Caller.SendAsync("ReceiveMessage", message);
        //}

        //public Task SendMessageToGroup(string message)
        //{
        //    return Clients.Group("SignalR Users").SendAsync("ReceiveMessage", message);
        //}

        //[HubMethodName("SendMessageToUser")]
        //public Task DirectMessage(string user, string message)
        //{
        //    return Clients.User(user).SendAsync("ReceiveMessage", message);
        //}
    }
}


//using AspNetDemo_SignalR.App_Lib;
//using Microsoft.AspNetCore.SignalR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AspNetDemo_SignalR.Hubs
//{
//    public class ChatHub : Hub
//    {
//        #region Data Members

//        static List<UserDetail> ConnectedUsers = new List<UserDetail>();
//        static List<MessageDetail> CurrentMessage = new List<MessageDetail>();

//        #endregion

//        #region Methods

//        public void Connect(string userName)
//        {
//            var id = Context.ConnectionId;


//            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
//            {
//                ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = userName });

//                // send to caller
//                Clients.Caller.onConnected(id, userName, ConnectedUsers, CurrentMessage);

//                // send to all except caller client
//                Clients.AllExcept(id).onNewUserConnected(id, userName);

//            }

//        }

//        public void SendMessageToAll(string userName, string message)
//        {
//            // store last 100 messages in cache
//            AddMessageinCache(userName, message);

//            // Broad cast message
//            Clients.All.messageReceived(userName, message);
//        }

//        public void SendPrivateMessage(string toUserId, string message)
//        {

//            string fromUserId = Context.ConnectionId;

//            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
//            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

//            if (toUser != null && fromUser != null)
//            {
//                // send to 
//                Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.UserName, message);

//                // send to caller user
//                Clients.Caller.sendPrivateMessage(toUserId, fromUser.UserName, message);
//            }

//        }

//        public override System.Threading.Tasks.Task OnDisconnected()
//        {
//            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
//            if (item != null)
//            {
//                ConnectedUsers.Remove(item);

//                var id = Context.ConnectionId;
//                Clients.All.onUserDisconnected(id, item.UserName);

//            }

//            return base.OnDisconnected();
//        }


//        #endregion

//        #region private Messages

//        private void AddMessageinCache(string userName, string message)
//        {
//            CurrentMessage.Add(new MessageDetail { UserName = userName, Message = message });

//            if (CurrentMessage.Count > 100)
//                CurrentMessage.RemoveAt(0);
//        }

//        #endregion
//    }
//}


/*
 
     public class TextHub : Hub
    {
        private static Dictionary<string, string> connectionsNgroup = new Dictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (connectionsNgroup.ContainsKey(Context.ConnectionId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connectionsNgroup[Context.ConnectionId]);
                connectionsNgroup.Remove(Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task BroadcastText(string text)
        {
            if (connectionsNgroup.ContainsKey(Context.ConnectionId))
            {
                await Clients.OthersInGroup(connectionsNgroup[Context.ConnectionId]).SendAsync("ReceiveText", text);
            }
        }

        public async Task JoinGroup(string group)
        {
            if (connectionsNgroup.ContainsKey(Context.ConnectionId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connectionsNgroup[Context.ConnectionId]);
                connectionsNgroup.Remove(Context.ConnectionId);
            }
            connectionsNgroup.Add(Context.ConnectionId, group);
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }
    }
 */

/*
 
 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKUserInbox.Hubs
{
    [Authorize(AuthenticationSchemes = "Bearer ", Roles = "Admin,Operator")]
    public class MainHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string user = Context.UserIdentifier;


            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }

        public Task SendPrivateMessage(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveMessage", message);
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public Task SendMessageToGroup(string message)
        {
            return Clients.Group("SignalR Users").SendAsync("ReceiveMessage", message);
        }

        [HubMethodName("SendMessageToUser")]
        public Task DirectMessage(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveMessage", message);
        }
    }
}

 
 */

/*
 
 
 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using SignalRChat.Common;

namespace SignalRChat
{
    public class ChatHub : Hub
    {
        #region Data Members

        static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        static List<MessageDetail> CurrentMessage = new List<MessageDetail>();

        #endregion

        #region Methods

        public void Connect(string userName)
        {
            var id = Context.ConnectionId;


            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = userName });

                // send to caller
                Clients.Caller.onConnected(id, userName, ConnectedUsers, CurrentMessage);

                // send to all except caller client
                Clients.AllExcept(id).onNewUserConnected(id, userName);

            }

        }

        public void SendMessageToAll(string userName, string message)
        {
            // store last 100 messages in cache
            AddMessageinCache(userName, message);

            // Broad cast message
            Clients.All.messageReceived(userName, message);
        }

        public void SendPrivateMessage(string toUserId, string message)
        {

            string fromUserId = Context.ConnectionId;

            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId) ;
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null && fromUser!=null)
            {
                // send to 
                Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.UserName, message); 

                // send to caller user
                Clients.Caller.sendPrivateMessage(toUserId, fromUser.UserName, message); 
            }

        }

        public override System.Threading.Tasks.Task OnDisconnected()
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.UserName);

            }

            return base.OnDisconnected();
        }

     
        #endregion

        #region private Messages

        private void AddMessageinCache(string userName, string message)
        {
            CurrentMessage.Add(new MessageDetail { UserName = userName, Message = message });

            if (CurrentMessage.Count > 100)
                CurrentMessage.RemoveAt(0);
        }

        #endregion
    }

}
 
 */
