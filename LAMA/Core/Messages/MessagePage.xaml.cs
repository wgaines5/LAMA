using System.Collections.ObjectModel;
using System.Web;
using System.Windows.Input;

namespace LAMA.Core.Messages
{
    public partial class MessagePage : ContentPage
    {
        public ChatViewModel ViewModel { get; private set; }

        public MessagePage()
        {
            InitializeComponent();
            ViewModel = new ChatViewModel();
            BindingContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            var query = HttpUtility.ParseQueryString(new Uri(Shell.Current.CurrentState.Location.OriginalString).Query);

            if (query["Question"] is string questionText && !string.IsNullOrWhiteSpace(questionText) &&
                query["Category"] is string categoryText && !string.IsNullOrWhiteSpace(categoryText))
            {
                ViewModel.ReceiveUserQuestion(questionText, categoryText);
            }
        }

        private void OnSendMessage(object sender, EventArgs e)
        {
            if (BindingContext is ChatViewModel chatViewModel)
            {
                chatViewModel.SendMessageCommand.Execute(null);
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
            SendMessageCommand = new Command(() => SendMessage());
        }

        public void ReceiveUserQuestion(string question, string category)
        {
            Messages.Add(new ChatMessage
            {
                Content = $"🩺 Category: {category}\n\n💬 Question: {question}",
                IsUserMessage = true
            });
            OnPropertyChanged(nameof(Messages)); 
        }

        private void SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(NewMessage))
            {
                Messages.Add(new ChatMessage
                {
                    Content = $"🗨️ You: {NewMessage}",
                    IsUserMessage = true
                });

                NewMessage = string.Empty;
                OnPropertyChanged(nameof(NewMessage));
                OnPropertyChanged(nameof(Messages));
            }
        }
    }

    public class ChatMessage
    {
        public string Content { get; set; }
        public bool IsUserMessage { get; set; }
    }
}