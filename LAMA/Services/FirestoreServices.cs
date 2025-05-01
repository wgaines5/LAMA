using Firebase.Auth.Requests;
using Google.Api;
using Google.Cloud.Firestore;
using LAMA.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LAMA.Services
{
    public class FirestoreServices
    {
        public FirestoreDb db;
        private HttpClient _httpClient;
        private readonly string _url;

        public async Task<List<Conversation>> GetUnassignedConversationsAsync()
        {
            string url = "https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/unassigned_queries/fcL8qgVXnlVeY7DIfiNk";

            var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<Conversation>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var parsed = JsonDocument.Parse(json);

            var conversations = new List<Conversation>();

            if (parsed.RootElement.TryGetProperty("documents", out var documents))
            {
                foreach (var doc in documents.EnumerateArray())
                {
                    var fields = doc.GetProperty("fields");
                    Conversation conversation = new Conversation
                    {
                        ConversationId = fields.GetProperty("conversationId").GetProperty("stringValue").GetString(),
                        ProviderId = fields.GetProperty("providerId").GetProperty("stringValue").GetString(),
                        LastUpdated = DateTime.Parse(fields.GetProperty("lastUpdated").GetProperty("timestampValue").GetString()),
                        LastMessage = ParseChatMessage(fields.GetProperty("lastMessage").GetProperty("mapValue").GetProperty("fields")),
                        ParticipantIds = ParseParticipantIds(fields),
                        ChatMessages = ParseChatMessages(fields),
                        Messages = ParseMessages(fields)
                    };
                    conversations.Add(conversation);
                }
            }
            return conversations;
        }

        private ChatMessage ParseChatMessage(JsonElement fields)
        {
            return new ChatMessage
            {
                Content = fields.GetProperty("content").GetProperty("stringValue").GetString(),
                SenderId = fields.GetProperty("senderId").GetProperty("stringValue").GetString(),
                ReceiverId = fields.GetProperty("recieverId").GetProperty("stringValue").GetString(),
                SentAt = fields.GetProperty("sentAt").GetProperty("timestampValue").GetString(),
                SessionId = fields.GetProperty("sessionId").GetProperty("stringValue").GetString()
            };
        }

        private List<ChatMessage> ParseChatMessages(JsonElement fields)
        {
            List<ChatMessage> chatMessages = new List<ChatMessage>();

            if (fields.TryGetProperty("chatMessages", out var chatMessagesField))
            {
                foreach (var field in chatMessagesField.GetProperty("arrayValue").GetProperty("values").EnumerateArray())
                {
                    var messageFields = field.GetProperty("mapValue").GetProperty("fields");
                    chatMessages.Add(ParseChatMessage(messageFields));
                }
            }
            return chatMessages;
        }

        private List<string> ParseParticipantIds(JsonElement fields) 
        {
            List<string> participantIds = new List<string>();

            if (fields.TryGetProperty("participantIds", out var participantField))
            {
                foreach (var idField in participantField.GetProperty("arrayValue").GetProperty("values").EnumerateArray())
                {
                    participantIds.Add(idField.GetProperty("stringValue").GetString());
                }
            }
            return participantIds;
        }

        private List<Message> ParseMessages(JsonElement fields)
        {
            List<Message> messages = new List<Message>();

            if (fields.TryGetProperty("messages", out var messagesField))
            {
                foreach (var messageField in messagesField.GetProperty("arrayValue").GetProperty("values").EnumerateArray())
                {
                    var msgFields = messageField.GetProperty("mapValue").GetProperty("fields");

                    Message message = new Message
                    {
                        SenderId = msgFields.GetProperty("senderId").GetProperty("stringValue").GetString(),
                        Content = msgFields.GetProperty("content").GetProperty("stringValue").GetString(),
                        Timestamp = DateTime.Parse(msgFields.GetProperty("timestamp").GetProperty("timestampValue").GetString()),
                        IsRead = msgFields.GetProperty("isRead").GetProperty("booleanValue").GetBoolean()
                    };
                    messages.Add(message);
                }
            }
            return messages;
        }

        public async Task<List<UnassignedMessage>> GetUnassignedAsync()
        {
            string url = "https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/unassigned_queries";
            _httpClient = new HttpClient();
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return new List<UnassignedMessage>();
            }

            string json = await response.Content.ReadAsStringAsync();
            var document = JsonDocument.Parse(json);

            List<UnassignedMessage> unassignedMessages = new List<UnassignedMessage>();

            foreach (var doc in document.RootElement.GetProperty("documents").EnumerateArray())
            {
                var fields = doc.GetProperty("fields");

                unassignedMessages.Add(new UnassignedMessage
                {
                    IsAssigned = fields.GetProperty("isAssigned").GetProperty("booleanValue").GetBoolean(),
                    Text = fields.GetProperty("message").GetProperty("stringValue").GetString(),
                    SenderId = fields.GetProperty("senderId").GetProperty("stringValue").GetString(),
                    Timestamp = DateTime.Parse(fields.GetProperty("timestamp").GetProperty("timestampValue").GetString())
                });
            }
            return unassignedMessages;
        }
    }
    [FirestoreData]
    public class SampleModel
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string Description { get; set; }
        [FirestoreProperty]
        public DateTime Created { get; set; }
    }

    public class DateTimeToTimeStampConverter : IFirestoreConverter<DateTime>
    {
        public object ToFirestore(DateTime value) => Google.Cloud.Firestore.Timestamp.FromDateTime(value.ToUniversalTime());

        public DateTime FromFirestore(object value)
        {
            if (value is Google.Cloud.Firestore.Timestamp timestamp)
            {
                return timestamp.ToDateTime();
            }
            throw new ArgumentException("Invalid value");
        }
    }

    public class ChatMessageConverter : IFirestoreConverter<ChatMessage>
    {
        public ChatMessage FromFirestore(object value)
        {
            var data = value as Dictionary<string, object>;
            return new ChatMessage
            {
                SenderId = data["SenderId"] as string,
                ReceiverId = data["ReceiverId"] as string,
                Content = data["Content"] as string,
                IsUserMessage = (bool)data["IsUserMessage"],
            };
        }

        public object ToFirestore(ChatMessage message)
        {
            return new Dictionary<string, object>
        {
            { "SenderId", message.SenderId },
            { "ReceiverId", message.ReceiverId },
            { "Content", message.Content },
            { "IsUserMessage", message.IsUserMessage },
        };
        }
    }
}
