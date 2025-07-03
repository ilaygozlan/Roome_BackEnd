using Microsoft.AspNetCore.SignalR;

namespace Roome_BackEnd.Hubs
{
    public class ChatHub : Hub
    {
       public override async Task OnConnectedAsync()
{
    var userId = GetUserIdFromContext();

    if (!string.IsNullOrEmpty(userId))
    {
        var existingConnections = ConnectionMapping.GetConnections(userId);
        foreach (var connId in existingConnections)
        {
            await Clients.Client(connId).SendAsync("ForceDisconnect"); 
        }

        ConnectionMapping.RemoveAll(userId);
        ConnectionMapping.Add(userId, Context.ConnectionId);

        Console.WriteLine($" User {userId} connected with ConnectionId {Context.ConnectionId}");
    }

    await base.OnConnectedAsync();
}


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = GetUserIdFromContext();

            if (!string.IsNullOrEmpty(userId))
            {
                ConnectionMapping.Remove(userId, Context.ConnectionId);
                Console.WriteLine($"User {userId} disconnected");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string toUserId, string message)
        {
            var senderId = GetUserIdFromContext();

            var targetConnections = ConnectionMapping.GetConnections(toUserId);

            foreach (var connectionId in targetConnections)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderId, message);
            }
        }

        private string GetUserIdFromContext()
        {
            return Context.GetHttpContext()?.Request?.Query["userId"].ToString() ?? string.Empty;
        }
    }
}
