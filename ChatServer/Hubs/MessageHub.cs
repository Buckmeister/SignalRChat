using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ChatServer.Hubs
{
    internal class MessageHub : Hub<IMessageClient>
    {
        private IHubConnectionEventLog _connectionLog;

        public MessageHub(IHubConnectionEventLog hubConnectionLog)
        {
            _connectionLog = hubConnectionLog;
        }
        
        public async Task SendMessage(string user, string message)
        {
            if (message.StartsWith("/"))
            {
                await SendMessage("Chat Server", message.Substring(1));
                return;
            }
            await Clients.All.ReceiveMessage(user, message);
        }

        public async Task SendMoodMessage(string user, string caption, byte[] imageData)
        {
            try
            {
                await Clients.All.ReceiveMoodMessage(user, caption, imageData);
            }
            catch (Exception ex)
            {
                await SendMessage("Error while relaying Mood Message:", ex.Message);
            }
        }

        public override Task OnConnectedAsync()
        {
            _connectionLog.AddEvent($"New Connection with ID: {Context.ConnectionId.ToString()}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connectionLog.AddEvent($"New Disconnect: {Context.ConnectionId.ToString()}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}