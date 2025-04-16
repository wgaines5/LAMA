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

namespace LAMA.Core.Messages;

public partial class MessagePage : ContentPage
{
	public MessagePage()
	{
		InitializeComponent();
		BindingContext = new ChatViewModel(new FirebaseService());
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

	public ChatViewModel(FirebaseService firebaseService)
	{
		_services = firebaseService;
		SendMessageCommand = new Command(async () => await SendMessage());
		Task.Run(async () => await RefreshMessageAsync());
	}

	private async Task SendMessage()
	{
		if (!string.IsNullOrWhiteSpace(NewMessage))
		{
			ChatMessage message = new ChatMessage
			{
				SenderId = "User1",
				ReceiverId = "User2",
				Content = NewMessage,
				IsUserMessage = true,
				SentAt = DateTime.UtcNow.ToString("o")
			};
			await _services.SendMessageAsync(message);
			Messages.Add(new ChatMessage { Content = NewMessage, IsUserMessage = true});
			NewMessage = string.Empty;
			OnPropertyChanged(nameof(NewMessage));
		}
	}

	public async Task RefreshMessageAsync()
	{
		var messages = await _services.GetMessageAsync();

		Messages.Clear();

		foreach (var message in messages.OrderBy(m => DateTime.Parse(m.SentAt)))
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
	public string SenderId { get; set; }

	public string ReceiverId { get; set; }

	public string Content { get; set; }

	public bool IsUserMessage { get; set; }

	public string SentAt { get; set; } = DateTime.UtcNow.ToString("o");

	public string SessionId { get; set; }
}


