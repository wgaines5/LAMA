using System.Collections.ObjectModel;
using LAMA.Core.Messages;
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
                categoryI.OnSelectionChanged = async (item) => await UpdateSelectedCategoriesInFirestore();
            }

            CategoriesList.ItemsSource = Categories;
            PendingMessagesList.ItemsSource = PendingMessages;
            UsersAnsweredCount.Text = UsersAnswered.ToString();

            _ = LoadUserProfileAsync();
        }

        private async Task LoadUserProfileAsync()
        {
            try
            {
                if (UserSession.Credential == null || UserSession.Credential.User == null)
                {
                    WelcomeMessage.Text = "No Authorization (Offline)";
                    WelcomeMessage.TextColor = Colors.Red;
                    return;
                }

                string uid = UserSession.Credential.User.Uid;
                string idToken = await UserSession.Credential.User.GetIdTokenAsync();

                string url = $"https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/medical_providers/{uid}?access_token={idToken}";

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {

                    string json = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(json);
                    JsonElement fields;
                    if (doc.RootElement.TryGetProperty("fields", out fields))
                    {
                        bool isVerified = fields.GetProperty("isVerified").GetProperty("booleanValue").GetBoolean();
                        string firstName = fields.GetProperty("firstName").GetProperty("stringValue").GetString();
                        if (isVerified)
                        {
                            WelcomeMessage.Text = $"Welcome, Dr. {firstName} ";
                            WelcomeMessage.TextColor = Colors.Green;
                        }
                        else
                        {
                            WelcomeMessage.Text = $"Welcome, Dr. {firstName} (Not Verified)";
                            WelcomeMessage.TextColor = Colors.OrangeRed;
                        }
                        if (fields.TryGetProperty("categories", out JsonElement categoryField))
                        {
                            if (categoryField.TryGetProperty("arrayValue", out JsonElement arrayValue) &&
                                arrayValue.TryGetProperty("values", out JsonElement values))
                            {
                                List<string> selectedCategories = values.EnumerateArray()
                                    .Select(v => v.GetProperty("stringValue").GetString())
                                    .ToList();

                                foreach (CategoryItem category in Categories)
                                {
                                    category.IsSelected = selectedCategories.Contains(category.Name);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                WelcomeMessage.Text = "Failed to load profile";
                WelcomeMessage.TextColor = Colors.Red;
            }
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Remove("userEmail");
            Preferences.Remove("userPassword");
            Shell.Current.GoToAsync("//SignInPage");
        }

        private async void OnOnlineToggleChanged(object sender, ToggledEventArgs e)
        {
            bool isOnline = e.Value;

            if (UserSession.Credential == null || UserSession.Credential.User == null)
                return;

            try
            {
                string uid = UserSession.Credential.User.Uid;
                string idToken = await UserSession.Credential.User.GetIdTokenAsync();

                string url = $"https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/medical_providers/{uid}?updateMask.fieldPaths=isOnline&access_token={idToken}";

                Dictionary<string, object> isOnlineField = new Dictionary<string, object>
                {
                    ["isOnline"] = new Dictionary<string, object>
                    {
                        ["booleanValue"] = isOnline
                    }
                };

                Dictionary<string, object> requestBody = new Dictionary<string, object>
                {
                    ["fields"] = isOnlineField
                };

                string json = JsonSerializer.Serialize(requestBody);

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Patch, url)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                HttpResponseMessage response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Error", "Failed to update online status.", "OK");
                }

                await LoadUserProfileAsync();
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
                await Shell.Current.GoToAsync($"{nameof(MessagePage)}?Question={Uri.EscapeDataString(message.Message)}");
                PendingMessages.Remove(message);
            }
        }

        private async Task UpdateSelectedCategoriesInFirestore()
        {
            if (UserSession.Credential == null || UserSession.Credential.User == null)
                return;

            string idToken = await UserSession.Credential.User.GetIdTokenAsync();
            string uid = UserSession.Credential.User.Uid;

            List<string> selected = Categories
                .Where(c => c.IsSelected)
                .Select(c => c.Name)
                .ToList();

            Dictionary<string, object> updateFields = new Dictionary<string, object>
            {
                ["categories"] = new
                {
                    arrayValue = new
                    {
                        values = selected.Select(name => new { stringValue = name }).ToList()
                    }
                }
            };

            string jsonPatch = JsonSerializer.Serialize(new { fields = updateFields });

            string url = $"https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/medical_providers/{uid}?access_token={idToken}&updateMask.fieldPaths=categories";

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(jsonPatch, Encoding.UTF8, "application/json")
            };

            await client.SendAsync(request);
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
