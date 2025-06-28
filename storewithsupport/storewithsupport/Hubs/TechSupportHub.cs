using Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace storewithsupport.Hubs;

public class TechSupportHub(ILogger<TechSupportHub> logger):Hub<IChatClient>
{
    public async Task JoinRoom(UserConnection connection)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);

        logger.LogInformation($"{connection.UserName} joined chat room: {connection.ChatRoom}");
    }

    public async Task SendMessage(UserConnection connection, string message)
    {
        await Clients.Group(connection.ChatRoom).ReceiveMessage(connection.UserName, message);
    }
}
public interface IChatClient
{
    Task ReceiveMessage(string username,string message);
}