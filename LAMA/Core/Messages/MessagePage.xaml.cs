#if ANDROID
using Android.App;
using AndroidX.Lifecycle;
#endif
using Google.Cloud.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LAMA.Services;
using LAMA.Auth;
using Firebase.Auth;
using System.Text.Json.Serialization;

namespace LAMA.Core.Messages;

public partial class MessagePage : ContentPage
{
	public MessagePage()
	{
		InitializeComponent();
		BindingContext = new ChatViewModel(new FirebaseService(), string.Empty);
	}

	public MessagePage(Conversation conversation)
	{
		InitializeComponent();
		BindingContext = new ChatViewModel(new FirebaseService(), conversation.ConversationId);

		UserSession.SessionId = conversation.ConversationId;
		UserSession.ProviderId = conversation.ProviderId;
	}

	private void OnSendMessage(object sender, EventArgs e)
	{
		if (BindingContext is ChatViewModel chatViewModel)
		{
			chatViewModel.SendMessageCommand.Execute(this);
		}
	}
}

public class ChatViewModel : BindableObject
{
	private FirebaseService _services;
	private string _newMessage;
	private string _uid;
	private string _sessionId;

	public ObservableCollection<ChatMessage> Messages { get; set; } = new ObservableCollection<ChatMessage>();
	public string NewMessage
	{
		get => _newMessage;

		set
		{
			if (_newMessage != value)
			{
				_newMessage = value;
				OnPropertyChanged();
			}
		}
	}
	public ICommand SendMessageCommand { get; }

	public ChatViewModel(FirebaseService firebaseService, string sessionId)
	{
		_services = firebaseService;
		_sessionId = sessionId;
		SendMessageCommand = new Command(async () => await SendMessage());

		if (UserSession.Credential != null)
		{
            _uid = UserSession.Credential.User.Uid;
        }
		
		Task.Run(async () => await RefreshMessageAsync());
	}

	private async Task SendMessage()
	{
		if (!string.IsNullOrWhiteSpace(NewMessage))
		{
			string recieverId = UserSession.ProviderId ?? "unassigned";
			string sessionId = UserSession.SessionId ?? GenerateSessionId(_uid, recieverId);
			ChatMessage message = new ChatMessage
			{
				SenderId = _uid,
				ReceiverId = recieverId,
				Content = NewMessage,
				IsUserMessage = true,
				SentAt = DateTime.UtcNow.ToString("o"),
				SessionId = _sessionId
			};

			if (recieverId == "unassigned")
			{
				Conversation conversation = new Conversation
				{
					ConversationId = 
					sessionId,
					LastMessage = message,
					LastUpdated = DateTime.UtcNow,
					ProviderId = recieverId
				};
				await _services.AddUnassignedAsync(conversation);
			}
			else
			{
                await _services.SendMessageAsync(message);
            }
			
			Messages.Add(new ChatMessage { Content = NewMessage, IsUserMessage = true});
			NewMessage = string.Empty;
			OnPropertyChanged(nameof(NewMessage));
		}
	}

	public async Task RefreshMessageAsync()
	{
		var messages = await _services.GetMessageAsync();

		var sessionMessages = messages
			.Where(m => m.SessionId == UserSession.SessionId)
			.OrderBy(m => DateTime.Parse(m.SentAt));

		Messages.Clear();

		foreach (var message in sessionMessages)
		{
			Messages.Add(message);
		}
	}

	private static string GenerateSessionId(string user1, string user2)
	{
		if (string.Compare(user1, user2) < 0)
		{
			return $"{user1}_{user2}";
		}
		else
		{
			return $"{user2}_{user1}";
		}
	}
}

public class ChatMessage
{
	[JsonPropertyName("senderId")]
	public string SenderId { get; set; }

	[JsonPropertyName("receiverId")]
	public string ReceiverId { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("isUserMessage")]
    public bool IsUserMessage { get; set; }

    [JsonPropertyName("sentAt")]
    public string SentAt { get; set; } = DateTime.UtcNow.ToString("o");

    [JsonPropertyName("sessionId")]
    public string SessionId { get; set; }
}


