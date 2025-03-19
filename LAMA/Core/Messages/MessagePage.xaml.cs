#if ANDROID
using Android.App;
using AndroidX.Lifecycle;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LAMA.Core.Messages;

public partial class MessagePage : ContentPage
{
	public MessagePage()
	{
		InitializeComponent();
		BindingContext = new ChatViewModel(new MessageService("https://localhost:5001/chathub"));

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
	private MessageService _messageService;
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

	public ChatViewModel(MessageService messageService)
	{
		_messageService = messageService;
		SendMessageCommand = new Command(async () => await SendMessage());

		_messageService.MessageReceived += (message) =>
		{
			Messages.Add(new ChatMessage { Content = message, IsUserMessage = false });
		};

		Task.Run(async () => await _messageService.ConnectAsync());
	}

	private async Task SendMessage()
	{
		if (!string.IsNullOrWhiteSpace(NewMessage))
		{
			await _messageService.SendMessageAsync(NewMessage);
			Messages.Add(new ChatMessage { Content = NewMessage, IsUserMessage = true});
			NewMessage = string.Empty;
			OnPropertyChanged(nameof(NewMessage));
		}
	}
}

public class ChatMessage
{
	public string Content { get; set; }
	public bool IsUserMessage { get; set; }

}


