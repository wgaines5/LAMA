using LAMA.Core.Messages;
using LAMA.Auth;
using System.Text.Json;
using System.Text;

namespace LAMA.Core.Categories
{
    public partial class AlternativeHPage : ContentPage
    {
        public AlternativeHPage()
        {
            InitializeComponent();
        }

        private async void OnAskQuestion(object sender, EventArgs e)
        {

            if (UserSession.CurrentUser == null)
            {

                UserSession.CurrentUser = await AuthServices.SignInAnonymouslyAsync();


                if (UserSession.CurrentUser == null)
                {
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error",
                        "Could not sign in anonymously. Please try again.",
                        "Ok");
                }
            }
            string questionText = QuestionEntry.Text;
            string selectedCategory = "Alternative & Holistic Medicine";
            string idSender = UserSession.CurrentUser.Uid;
            string idSession = $"{UserSession.CurrentUser.Uid}_{DateTime.UtcNow:yyyyMMddHHmmss}";

            if (string.IsNullOrWhiteSpace(questionText) || string.IsNullOrWhiteSpace(selectedCategory))
            {
                await DisplayAlert("Error", "Please enter a question and select a category.", "OK");
                return;
            }

            var newMessage = new
            {
                message = questionText,
                timestamp = DateTime.UtcNow.ToString("o"),
                senderId = idSender,
                isAssigned = false,
                sessionId = idSession,
                category = selectedCategory
            };

            // Send to firebase
            var json = JsonSerializer.Serialize(newMessage);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await new HttpClient().PutAsync($"https://lama-60ddc-default-rtdb.firebaseio.com/queries/{idSession}.json", content);
            response.EnsureSuccessStatusCode();

            var secondResponse = await new HttpClient().PostAsync($"https://lama-60ddc-default-rtdb.firebaseio.com/{idSession}/messages.json", content);
            secondResponse.EnsureSuccessStatusCode();

            QuestionEntry.Text = string.Empty;

            await Shell.Current.GoToAsync($"MessagePage?SenderId={Uri.EscapeDataString(idSender)}&SessionId={Uri.EscapeDataString(idSession)}&Category={Uri.EscapeDataString(selectedCategory)}");
        }
        
    }
}