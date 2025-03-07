using System.Collections.ObjectModel;
using System.Windows.Input;
using LAMA.Core.Messages;
using Microsoft.AspNetCore.SignalR.Client;

namespace LAMA.Core;

public partial class MessagesPage : ContentPage
{
	public MessagesPage()
	{
		InitializeComponent();
		BindingContext = new ChatViewModel(new MessageService("https://localhost:5001/chathub"));
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
