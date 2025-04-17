using LAMA.Auth;
using LAMA.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LAMA.Services
{
    public class FirebaseService
    {
        private readonly HttpClient _httpClient;
        private readonly string _url = "https://lama-60ddc-default-rtdb.firebaseio.com";
        private readonly string _token;

        public FirebaseService()
        {
            _httpClient = new HttpClient();
            _token = UserSession.Token;
        }

        public async Task SendMessageAsync(ChatMessage message)
        {
            string json = JsonSerializer.Serialize(message);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_url}/messages.json?auth={_token}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<ChatMessage>> GetMessageAsync()
        {
            var response = await _httpClient.GetAsync($"{_url}?auth={_token}");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Dictionary<string, ChatMessage>>(json)?.Values.ToList() ?? new List<ChatMessage>();
            }
            return new List<ChatMessage>();
        }
    }
}
