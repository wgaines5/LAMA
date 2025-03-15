using Microsoft.AspNetCore.SignalR;

namespace SignalRHub
{
    public class ChatHub : Hub
    {
        public async Task SendMessageAsync(string message)
        {
            Console.WriteLine(message);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
