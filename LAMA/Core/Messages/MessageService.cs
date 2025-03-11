using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAMA.Core.Messages
{
    public class MessageService
    {
        private HubConnection _hubConnection;
        public event Action<string>? MessageReceived;

        public MessageService(string url)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();

            _hubConnection.On<string>("ReceiveMessage", message =>
            {
                MessageReceived?.Invoke(message);
            });
        }

        public async Task ConnectAsync()
        {
            if (_hubConnection == null)
            {
                await _hubConnection.StartAsync();
            }
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.StopAsync();
            }
        }

        public async Task SendMessageAsync(string message)
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.SendAsync(message);
            }
        }
    }
}
