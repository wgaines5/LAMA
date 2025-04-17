using System.Collections.ObjectModel;
using Google.Cloud.Firestore;
using Microsoft.Maui.Controls;
using System.Linq;
using System;
using System.Text.Json;


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
                JsonElement fields;
                if (doc.RootElement.TryGetProperty("fields", out fields))
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
