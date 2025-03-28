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
		BindingContext = new ChatViewModel(new FirestoreServices());

		(BindingContext as ChatViewModel).Messages.CollectionChanged += (sender, e) =>
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				AutoScoll();
			}
		};
	}

	private void OnSendMessage(object sender, EventArgs e)
	{
		if (BindingContext is ChatViewModel chatViewModel)
		{
			chatViewModel.SendMessageCommand.Execute(this);
		}
	}

	private async void AutoScoll()
	{
		if (MessageList.ItemsSource is ObservableCollection<ChatMessage> messages && messages.Count > 0)
		{
			this.Dispatcher.Dispatch(() =>
			{
				MessageList.ScrollTo(messages[messages.Count - 1], position: ScrollToPosition.End, animate: true);
			});
        }
	}
}

public class ChatViewModel : BindableObject
{
	private FirestoreServices _services;
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

	public ChatViewModel(FirestoreServices firestoreServices)
	{
		_services = firestoreServices;
		SendMessageCommand = new Command(async () => await SendMessage());
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
				SentAt = Timestamp.FromDateTime(DateTime.UtcNow)
			};
			await _services.SendMessage(message);
			Messages.Add(new ChatMessage { Content = NewMessage, IsUserMessage = true});
			NewMessage = string.Empty;
			OnPropertyChanged(nameof(NewMessage));
		}
	}
}

[FirestoreData(ConverterType = typeof(ChatMessageConverter))]
public class ChatMessage
{
	[FirestoreProperty]
	public string SenderId { get; set; }

	[FirestoreProperty]
	public string ReceiverId { get; set; }

	[FirestoreProperty]
	public string Content { get; set; }

	[FirestoreProperty]
	public bool IsUserMessage { get; set; }

	[FirestoreProperty]
	public Timestamp SentAt { get; set; } = Timestamp.FromDateTime(DateTime.UtcNow);

}


