#if ANDROID
using Android.App;
using AndroidX.Lifecycle;
#endif

using LAMA.Auth;
using LAMA.Core.Profile;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace LAMA.Core.Messages;

[QueryProperty(nameof(SenderId), "SenderId")]
[QueryProperty(nameof(SessionId), "SessionId")]
[QueryProperty(nameof(Category), "Category")]
public partial class MessagePage : ContentPage
{

    LAMA.Core.Profile.User _currentUser;

    public string SenderId { get; set; }
    public string SessionId { get; set; }
    public string Category {  get; set; }

    public ObservableCollection<MessageItem> Messages { get; set; } = new();

    private CancellationTokenSource _pollingToken;

    public MessagePage()
    {
        InitializeComponent();

        BindingContext = this;
    }

    //protected override async void OnAppearing()
    //{
    //    base.OnAppearing();

    //    // Clear existing messages to avoid duplicates when navigating back to this page
    //    Messages.Clear();

    //    // Check that a valid SenderId was provided
    //    if (!string.IsNullOrEmpty(SenderId))
    //    {
    //        try
    //        {
    //            // Attempt to load messages associated with the provided SenderId
    //            var userMessages = await LoadConversationForUserAsync(SenderId);

    //            if (userMessages != null && userMessages.Any())
    //            {
    //                // Add the messages to the bound ObservableCollection
    //                foreach (var msg in userMessages)
    //                    Messages.Add(msg);

    //                // Debug output to confirm success
    //                Console.WriteLine($"Loaded {Messages.Count} message(s) for SenderId: {SenderId}");
    //            }
    //            else
    //            {
    //                Console.WriteLine("No messages found for this SenderId.");
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Error retrieving messages: {ex.Message}");
    //        }
    //    }
    //    else
    //    {
    //        Console.WriteLine("SenderId was null or empty.");
    //    }
    //}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _pollingToken = new CancellationTokenSource();
        StartPollingAsync(_pollingToken.Token);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _pollingToken?.Cancel();
    }

    public async Task<List<MessageItem>> LoadQueryForUserAsync(string senderId)
    {
        try
        {
            var response = await new HttpClient().GetStringAsync("https://lama-60ddc-default-rtdb.firebaseio.com/queries.json");

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

    public async Task<List<MessageItem>> LoadConversationForUserAsync(string senderId, string sessionId)
    {
        try
        {
            var response = await new HttpClient().GetStringAsync($"https://lama-60ddc-default-rtdb.firebaseio.com/{sessionId}/messages.json");

            var allMessages = JsonConvert.DeserializeObject<Dictionary<string, MessageItem>>(response);

            if (allMessages == null)
                return new List<MessageItem>();

            var filtered = allMessages.Values
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

        if (UserSession.CurrentUser == null)
        {

            UserSession.CurrentUser = await AuthServices.SignInAnonymouslyAsync();
            _currentUser = UserSession.CurrentUser;

            if (UserSession.CurrentUser == null)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error",
                    "Could not sign in anonymously. Please try again.",
                    "Ok");
            }
        }
        
        var realtimeMessage = new
        {
            message = messageText,
            timestamp = DateTime.UtcNow.ToString("o"),
            senderId = _currentUser.Uid,
            isAssigned = false,
            profilePic = _currentUser.ProfilePictureUrl,
            sessionId = SessionId
        };

        string jsonRealtimeBody = System.Text.Json.JsonSerializer.Serialize(realtimeMessage);

        try
        {

            if (string.IsNullOrEmpty(SenderId))
            {
                // No user ID — treat as a new unassigned message
                string unassignedUrl = $"https://lama-60ddc-default-rtdb.firebaseio.com/queries.json";
                string queryConvoUrl = $"https://lama-60ddc-default-rtdb.firebaseio.com/{SessionId}/messages.json";
                await PostToRealtimeDatabaseAsync(unassignedUrl, jsonRealtimeBody);
                await PostToRealtimeDatabaseAsync(queryConvoUrl, jsonRealtimeBody);

                var messages = await LoadConversationForUserAsync(SenderId, SessionId);
                foreach (var msg in messages)
                {
                    Messages.Add(msg); // Add updated messages
                }

            }
            else
            {
                // Existing conversation — append to conversation path
                string conversationUrl = $"https://lama-60ddc-default-rtdb.firebaseio.com/{SessionId}/messages.json";
                await PostToRealtimeDatabaseAsync(conversationUrl, jsonRealtimeBody);

                Messages.Clear();
                var refreshedMessages = await LoadConversationForUserAsync(SenderId, SessionId);
                foreach (var msg in refreshedMessages)
                {
                    Messages.Add(msg); // Add updated messages
                }

            }

            MessageEntry.Text = string.Empty;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to send message: {ex.Message}", "OK");
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

    private async void StartPollingAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (!string.IsNullOrEmpty(SenderId))
            {
                try
                {
                    // Attempt to load messages associated with the provided SenderId
                    var userMessages = await LoadConversationForUserAsync(SenderId, SessionId);

                    if (userMessages != null && userMessages.Any())
                    {
                        // Add the messages to the bound ObservableCollection
                        //foreach (var msg in userMessages)
                        //    Messages.Add(msg);

                        var existingTimestamps = Messages.Select(m => m.Timestamp).ToHashSet();

                        var newMessages = userMessages
                            .Where(m => !existingTimestamps.Contains(m.Timestamp))
                            .OrderBy(m => DateTime.Parse(m.Timestamp))
                            .ToList();

                        if (newMessages.Any())
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                foreach (var message in newMessages)
                                {
                                    Messages.Add(message);
                                }
                            });
                        }

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

            await Task.Delay(1000);
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