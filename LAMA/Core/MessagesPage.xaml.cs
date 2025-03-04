using System.Collections.ObjectModel;
using System.Web;
using Microsoft.Maui.Controls;

namespace LAMA.Core
{
    public partial class MessagesPage : ContentPage
    {
        public ObservableCollection<string> ChatMessages { get; set; } = new ObservableCollection<string>();
        

        public MessagesPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            var query = HttpUtility.ParseQueryString(new Uri(Shell.Current.CurrentState.Location.OriginalString).Query);

            if (query["Question"] is string questionText && !string.IsNullOrWhiteSpace(questionText))
            {
                ChatMessages.Add($"User: {questionText}");
            }
        }

        private void OnSendMessage(object sender, EventArgs e)
        {
            Entry chatEntry = (Entry)FindByName("ChatEntry");

            if (chatEntry != null && !string.IsNullOrWhiteSpace(chatEntry.Text))
            {
                ChatMessages.Add($"User: {chatEntry.Text}");
                chatEntry.Text = ""; // clear input after sending
            }
        }
    }
}