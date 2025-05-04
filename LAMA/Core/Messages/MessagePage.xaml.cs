#if ANDROID
using Android.App;
using AndroidX.Lifecycle;
#endif

using LAMA.Auth;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace LAMA.Core.Messages;

[QueryProperty(nameof(SenderId), "SenderId")]
public partial class MessagePage : ContentPage
{

    LAMA.Core.Profile.User _currentUser = UserSession.CurrentUser;

    public string SenderId { get; set; }

    public ObservableCollection<MessageItem> Messages { get; set; } = new();
    
    public MessagePage()
    {
        InitializeComponent();

        BindingContext = this;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Debug output to ensure SenderId is properly passed to the page
        Console.WriteLine($"Received SenderId: {SenderId}");

        // Clear existing messages to avoid duplicates when navigating back to this page
        Messages.Clear();

        // Check that a valid SenderId was provided
        if (!string.IsNullOrEmpty(SenderId))
        {
            try
            {
                // Attempt to load messages associated with the provided SenderId
                var userMessages = await LoadMessagesForUserAsync(SenderId);

                if (userMessages != null && userMessages.Any())
                {
                    // Add the messages to the bound ObservableCollection
                    foreach (var msg in userMessages)
                        Messages.Add(msg);

                    // Debug output to confirm success
                    Console.WriteLine($"Loaded {Messages.Count} message(s) for SenderId: {SenderId}");
                }
                else
                {
                    Console.WriteLine("No messages found for this SenderId.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving messages: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("SenderId was null or empty.");
        }
    }

    public async Task<List<MessageItem>> LoadMessagesForUserAsync(string senderId)
    {
        try
        {
            var response = await new HttpClient().GetStringAsync("https://lama-60ddc-default-rtdb.firebaseio.com/unassigned_queries.json");

            var allMessages = JsonConvert.DeserializeObject<Dictionary<string, MessageItem>>(response);

            if (allMessages == null)
                return new List<MessageItem>();

            var filtered = allMessages.Values
                .Where(m => m.SenderId == senderId)
                .OrderBy(m => DateTime.Parse(m.Timestamp))
                .ToList();

            return filtered;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading user messages: {ex.Message}");
            return new List<MessageItem>();
        }
    }


    private async void OnSendMessage(object sender, EventArgs e)
    {

        string messageText = MessageEntry.Text?.Trim();

        if (string.IsNullOrEmpty(messageText))
            return;

        var messageData = new
        {
            fields = new
            {
                message = new { stringValue = messageText },
                timestamp = new { timestampValue = DateTime.UtcNow.ToString("o") },
                senderId = new { stringValue = _currentUser.Uid },
                isAssigned = new { booleanValue = false }
            }
        };

        var realtimeMessage = new
        {
            message = messageText,
            timestamp = DateTime.UtcNow.ToString("o"),
            senderId = _currentUser.Uid,
            isAssigned = false
        };


        string jsonFirestoreBody = System.Text.Json.JsonSerializer.Serialize(messageData);
        string jsonRealtimeBody = System.Text.Json.JsonSerializer.Serialize(realtimeMessage);


        string userConvoUrl = $"https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/users/{_currentUser.Uid}/conversations";
        string unassignedUrl = "https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/unassigned_queries";
        string unassignedUrlRealtime = "https://lama-60ddc-default-rtdb.firebaseio.com/unassigned_queries.json";

        try
        {
            await PostToFirestoreAsync(userConvoUrl, jsonFirestoreBody);
            await PostToFirestoreAsync(unassignedUrl, jsonFirestoreBody);
            // Post to Realtime Database (for live delivery to providers)
            await PostToRealtimeDatabaseAsync(unassignedUrlRealtime, jsonRealtimeBody);

            MessageEntry.Text = string.Empty;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to send message: {ex.Message}", "OK");
        }

    }

    private async Task PostToFirestoreAsync(string url, string jsonBody)
    {
        using var httpClient = new HttpClient();
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Firestore error: {response.StatusCode} -{await response.Content.ReadAsStringAsync()}");
        }
    }

    private async Task PostToRealtimeDatabaseAsync(string url, string jsonBody)
    {
        using (var client = new HttpClient())
        {
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Realtime DB Error: {response.StatusCode}");
            }
        }
    }


}























//	public MessagePage(Conversation conversation)
//	{
//		InitializeComponent();
//		BindingContext = new ChatViewModel(new FirebaseService(), conversation.ConversationId);

//		UserSession.SessionId = conversation.ConversationId;
//		UserSession.ProviderId = conversation.ProviderId;
//	}

//public class ChatViewModel : BindableObject
//{
//	private FirebaseService _services;
//	private string _newMessage;
//	private string _uid;
//	private string _sessionId;

//	public ObservableCollection<ChatMessage> Messages { get; set; } = new ObservableCollection<ChatMessage>();
//	public string NewMessage
//	{
//		get => _newMessage;

//		set
//		{
//			if (_newMessage != value)
//			{
//				_newMessage = value;
//				OnPropertyChanged();
//			}
//		}
//	}

//	public ICommand SendMessageCommand { get; }

//	public ChatViewModel(FirebaseService firebaseService, string sessionId)
//	{
//		_services = firebaseService;
//		_sessionId = sessionId;
//		SendMessageCommand = new Command(async () => await SendMessage());

//		if (UserSession.Credential != null)
//		{
//            _uid = UserSession.Credential.User.Uid;
//        }

//		Task.Run(async () => await RefreshMessageAsync());
//	}

//	private async Task SendMessage()
//	{
//		if (!string.IsNullOrWhiteSpace(NewMessage))
//		{
//			string recieverId = UserSession.ProviderId ?? "unassigned";
//			string sessionId = UserSession.SessionId ?? GenerateSessionId(_uid, recieverId);
//			ChatMessage message = new ChatMessage
//			{
//				SenderId = _uid,
//				ReceiverId = recieverId,
//				Content = NewMessage,
//				IsUserMessage = true,
//				SentAt = DateTime.UtcNow.ToString("o"),
//				SessionId = _sessionId
//			};

//			if (recieverId == "unassigned")
//			{
//				Conversation conversation = new Conversation
//				{
//					ConversationId = 
//					sessionId,
//					LastMessage = message,
//					LastUpdated = DateTime.UtcNow,
//					ProviderId = recieverId
//				};
//				await _services.AddUnassignedAsync(conversation);
//			}
//			else
//			{
//                await _services.SendMessageAsync(message);
//            }

//			Messages.Add(new ChatMessage { Content = NewMessage, IsUserMessage = true});
//			NewMessage = string.Empty;
//			OnPropertyChanged(nameof(NewMessage));
//		}
//	}

//	public async Task RefreshMessageAsync()
//	{
//		var messages = await _services.GetMessageAsync();

//		var sessionMessages = messages
//			.Where(m => m.SessionId == UserSession.SessionId)
//			.OrderBy(m => DateTime.Parse(m.SentAt));

//		Messages.Clear();

//		foreach (var message in sessionMessages)
//		{
//			Messages.Add(message);
//		}
//	}

//	private static string GenerateSessionId(string user1, string user2)
//	{
//		if (string.Compare(user1, user2) < 0)
//		{
//			return $"{user1}_{user2}";
//		}
//		else
//		{
//			return $"{user2}_{user1}";
//		}
//	}



//public class ChatMessage
//{
//	[JsonPropertyName("senderId")]
//	public string SenderId { get; set; }

//	[JsonPropertyName("receiverId")]
//	public string ReceiverId { get; set; }

//    [JsonPropertyName("content")]
//    public string Content { get; set; }

//    [JsonPropertyName("isUserMessage")]
//    public bool IsUserMessage { get; set; }

//    [JsonPropertyName("sentAt")]
//    public string SentAt { get; set; } = DateTime.UtcNow.ToString("o");

//    [JsonPropertyName("sessionId")]
//    public string SessionId { get; set; }
//}













//OnSendMessageBody
//if (BindingContext is ChatViewModel chatViewModel)
//{
//    chatViewModel.SendMessageCommand.Execute(this);
//}


//Constructor Body
//BindingContext = new ChatViewModel(new FirebaseService(), string.Empty);