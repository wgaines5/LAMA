using System.Collections.ObjectModel;
using System.Text.Json;
using System.IO;
using Microsoft.Maui.Controls;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace LAMA.Auth
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<string> Categories { get; set; }

        public MainPage()
        {
            InitializeComponent();

            Categories = new ObservableCollection<string>
            {
                "General Health",
                "Mental Health",
                "Sexual Health",
                "Chronic Conditions & Autoimmune",
                "Medication & Drug Interactions",
                "Alternative & Holistic Medicine"
            };

            BindingContext = this;

            _ = LoadTipsFromFirestoreAsync();
            _ = LoadMProvidersAsync();
        }

        private async Task LoadTipsFromFirestoreAsync()
        {
            HttpClient client = new HttpClient();
            string url = "https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/medicaladvice/fcL8qgVXnlVeY7DIfiNk";

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JsonDocument doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("fields", out JsonElement fields))
                {
                    foreach (JsonProperty field in fields.EnumerateObject())
                    {
                        if (field.Value.TryGetProperty("stringValue", out JsonElement value))
                        {
                            string tip = value.GetString();

                            Frame tipFrame = new Frame
                            {
                                CornerRadius = 10,
                                BackgroundColor = Color.FromArgb("#30cfcb"),
                                Padding = 10,
                                WidthRequest = 180,
                                Content = new Label
                                {
                                    Text = tip,
                                    FontSize = 14,
                                    TextColor = Colors.Black
                                }
                            };

                            MedicalAdviceTipsLayout.Children.Add(tipFrame);
                        }
                    }
                }
            }
            else
            {
                await DisplayAlert("Error", "Could not load medical tips from Firestore.", "OK");
            }
        }

        private async Task LoadMProvidersAsync()
        {
            HttpClient client = new HttpClient();
            string url = "https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/medical_providers";

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JsonDocument doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("documents", out JsonElement documents))
                {
                    foreach (JsonElement docItem in documents.EnumerateArray())
                    {
                        if (docItem.TryGetProperty("fields", out JsonElement fields))
                        {
                            bool isVerified = fields.TryGetProperty("isVerified", out JsonElement verifiedEl) &&
                                              verifiedEl.TryGetProperty("booleanValue", out JsonElement verifiedVal) &&
                                              verifiedVal.GetBoolean();

                            bool isOnline = fields.TryGetProperty("isOnline", out JsonElement onlineEl) &&
                                            onlineEl.TryGetProperty("booleanValue", out JsonElement onlineVal) &&
                                            onlineVal.GetBoolean();

                            if (isVerified && isOnline)
                            {
                                string fullName = "Doctor";
                                string base64Image = "";

                                if (fields.TryGetProperty("firstName", out JsonElement firstNameEl) &&
                                    firstNameEl.TryGetProperty("stringValue", out JsonElement firstNameVal))
                                {
                                    fullName = $"Dr. {firstNameVal.GetString()}";
                                }

                                if (fields.TryGetProperty("profileImageBase64", out JsonElement imageEl) &&
                                    imageEl.TryGetProperty("stringValue", out JsonElement imageVal))
                                {
                                    base64Image = imageVal.GetString();
                                }

                                if (!string.IsNullOrEmpty(base64Image))
                                {
                                    byte[] imageBytes = Convert.FromBase64String(base64Image);
                                    ImageSource profileImgSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));

                                    Frame profileFrame = new Frame
                                    {
                                        CornerRadius = 15,
                                        BackgroundColor = Color.FromArgb("#34485a"),
                                        Padding = 10,
                                        Content = new VerticalStackLayout
                                        {
                                            HorizontalOptions = LayoutOptions.Center,
                                            Spacing = 5,
                                            Children =
                                            {
                                                new Image
                                                {
                                                    Source = profileImgSource,
                                                    WidthRequest = 90,
                                                    HeightRequest = 90
                                                },
                                                new Label
                                                {
                                                    Text = fullName,
                                                    FontSize = 16,
                                                    FontAttributes = FontAttributes.Bold,
                                                    HorizontalOptions = LayoutOptions.Center
                                                }
                                            }
                                        }
                                    };

                                    MPLayout.Children.Add(profileFrame);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                await DisplayAlert("Error", "Failed to load medical professionals.", "OK");
            }
        }

        private async void OnMPSignupClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignUpPage");
        }

        private async void OnUserSignupClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//UsrSignUp");
        }

        private async void OnAskQuestion(object sender, EventArgs e)
        {
            string questionText = QuestionEntry.Text;
            string selectedCategory = CategoryPicker.SelectedItem as string;

            if (string.IsNullOrWhiteSpace(questionText) || string.IsNullOrWhiteSpace(selectedCategory))
            {
                await DisplayAlert("Error", "Please enter a question and select a category.", "OK");
                return;
            }

            QuestionEntry.Text = string.Empty;
            CategoryPicker.SelectedIndex = -1;

            await Shell.Current.GoToAsync($"MessagePage?Question={Uri.EscapeDataString(questionText)}&Category={Uri.EscapeDataString(selectedCategory)}");
        }
    }
}