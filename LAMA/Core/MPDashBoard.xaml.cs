using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace LAMA.Core
{
    public partial class MPDashBoard : ContentPage
    {
        public ObservableCollection<CategoryItem> Categories { get; set; }
        public ObservableCollection<MessageItem> PendingMessages { get; set; }
        public int UsersAnswered { get; set; }

        public MPDashBoard()
        {
            InitializeComponent();
            BindingContext = this;

            Categories = new ObservableCollection<CategoryItem>
            {
                new CategoryItem { Name = "General Health", IsSelected = false },
                new CategoryItem { Name = "Mental Health", IsSelected = false },
                new CategoryItem { Name = "Sexual & Reproductive Health", IsSelected = false },
                new CategoryItem { Name = "Chronic Conditions", IsSelected = false },
                new CategoryItem { Name = "Medication & Drug Interactions", IsSelected = false },
                new CategoryItem { Name = "Alternative & Holistic Medicine", IsSelected = false }
            };

            PendingMessages = new ObservableCollection<MessageItem>
            {
                new MessageItem { Message = "Patient: I need help with anxiety." },
                new MessageItem { Message = "Patient: What are the side effects of my medication?" },
                new MessageItem { Message = "Patient: How do I manage my diabetes better?" }
            };

            UsersAnswered = 123; // example count

            
            CategoriesList.ItemsSource = Categories;
            PendingMessagesList.ItemsSource = PendingMessages;
            UsersAnsweredCount.Text = UsersAnswered.ToString();
        }

        private void OnOnlineToggleChanged(object sender, ToggledEventArgs e)
        {
            bool isOnline = e.Value;
            WelcomeMessage.Text = isOnline ? "Welcome, Doctor! (Online)" : "Welcome, Doctor! (Offline)";
            WelcomeMessage.TextColor = isOnline ? Colors.Green : Colors.Red;
        }

        private async void OnReplyClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is MessageItem message)
            {
                await Shell.Current.GoToAsync($"{nameof(MessagesPage)}?Question={Uri.EscapeDataString(message.Message)}");
                PendingMessages.Remove(message);
            }
        }
    }

    public class CategoryItem
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class MessageItem
    {
        public string Message { get; set; }
    }
}