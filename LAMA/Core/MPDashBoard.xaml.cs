using System.Collections.ObjectModel;
using LAMA.Core.Messages;
using Firebase.Auth.Providers;
using Firebase.Auth;
using System.Text.Json;
using LAMA.Auth;
using System.ComponentModel;
using System.Text;


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

            Categories =
            [
                new() { Name = "General Health", IsSelected = false },
                new() { Name = "Mental Health", IsSelected = false },
                new() { Name = "Sexual & Reproductive Health", IsSelected = false },
                new() { Name = "Chronic Conditions", IsSelected = false },
                new() { Name = "Medication & Drug Interactions", IsSelected = false },
                new() { Name = "Alternative & Holistic Medicine", IsSelected = false }
            ];

            PendingMessages = new ObservableCollection<MessageItem>
            {
                new MessageItem { Message = "Patient: I need help with anxiety." },
                new MessageItem { Message = "Patient: What are the side effects of my medication?" },
                new MessageItem { Message = "Patient: How do I manage my diabetes better?" }
            };

            UsersAnswered = 0; // example count

            foreach (CategoryItem categoryI in Categories)
            {
                categoryI.OnSelectionChanged = async (item) => await UpdateSelectedCategoriesInFirebase();
            }

            CategoriesList.ItemsSource = Categories;
            PendingMessagesList.ItemsSource = PendingMessages;
            UsersAnsweredCount.Text = UsersAnswered.ToString();
        }

        private async Task InitializeUserSession()
        {
            SignInViewModel viewModel = new SignInViewModel(new FirebaseAuthClient(new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyDiAuutGePttuNIoUxGy2Ok6NDcqGoh74k",
                AuthDomain = "lama-60ddc.firebaseapp.com",
                Providers = new FirebaseAuthProvider[] { new EmailProvider() }
            }));

            await viewModel.UserActive();
        }

        private async void OnOnlineToggleChanged(object sender, ToggledEventArgs e)
        {
            bool isOnline = e.Value;

            try
            {
                if (UserSession.Credential == null || UserSession.Credential.User == null)
                {
                    WelcomeMessage.Text = "No Authorization (Offline)";
                    WelcomeMessage.TextColor = Colors.Red;
                    return;
                }

                string idToken = await UserSession.Credential.User.GetIdTokenAsync();
                string uid = UserSession.Credential.User.Uid;

                string url = $"https://lama-60ddc-default-rtdb.firebaseio.com/medical_providers/{uid}.json?auth={idToken}";

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using JsonDocument doc = JsonDocument.Parse(json);
                    JsonElement root = doc.RootElement;

                    string firstName = root.GetProperty("firstName").GetString();
                    WelcomeMessage.Text = isOnline
                        ? $"Welcome, Dr. {firstName} (Online)"
                        : $"Welcome, Dr. {firstName} (Offline)";
                    WelcomeMessage.TextColor = isOnline ? Colors.Green : Colors.Red;
                }
                else
                {
                    await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                }
            }
            catch
            {
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
        }

        private async void OnReplyClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is MessageItem message)
            {
                await Shell.Current.GoToAsync($"{nameof(InboxPage)}?Question={Uri.EscapeDataString(message.Message)}");
                PendingMessages.Remove(message);
            }
        }
    

    private async Task UpdateSelectedCategoriesInFirebase()
        {
            if (UserSession.Credential == null || UserSession.Credential.User == null)
                return;

            string idToken = await UserSession.Credential.User.GetIdTokenAsync();
            string uid = UserSession.Credential.User.Uid;

            List<string> selected = Categories
                .Where(c => c.IsSelected)
                .Select(c => c.Name)
                .ToList();

            string url = $"https://lama-60ddc-default-rtdb.firebaseio.com/medical_providers/{uid}/categories.json?auth={idToken}";
            HttpClient client = new HttpClient();

            if (selected.Count == 0)
            {
                await client.DeleteAsync(url);
            }
            else
            {
                string json = JsonSerializer.Serialize(selected);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                await client.SendAsync(request);
            }
        }
    }

    public class CategoryItem : INotifyPropertyChanged
    {
        public required string Name { get; set; }
        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));

                    // Trigger Firebase update
                    _ = OnSelectionChanged?.Invoke(this);
                }
            }
        }

        public Func<CategoryItem, Task> OnSelectionChanged { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class MessageItem
    {
        public string Message { get; set; }
    }
}