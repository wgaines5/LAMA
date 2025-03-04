using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LAMA.Core.Messages;

public partial class InboxPage : ContentPage
{
	public InboxPage()
	{
		InitializeComponent();
		BindingContext = new ChatViewModel();
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

	public ChatViewModel()
	{
		SendMessageCommand = new Command(SendMessage);
	}

	private void SendMessage()
	{
		if (!string.IsNullOrWhiteSpace(NewMessage))
		{
			Messages.Add(new ChatMessage { Content = NewMessage, IsUserMessage = true});
			NewMessage = string.Empty;
			OnPropertyChanged(nameof(NewMessage));

			// Simulate response
			Messages.Add(new ChatMessage { Content = "Thanks for your message.", IsUserMessage = false});
		}
	}
}

public class ChatMessage
{
	public string Content { get; set; }
	public bool IsUserMessage { get; set; }
}
