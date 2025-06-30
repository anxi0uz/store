using Microsoft.AspNetCore.SignalR.Client;

namespace Core.Models;

public record AdminConnectionInfo(string UserName, string ChatRoom, HubConnection Connection);