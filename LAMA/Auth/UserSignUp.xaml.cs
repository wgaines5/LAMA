using System.Text;
using System.Text.Json;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Google.Cloud.Firestore;
using LAMA.Core.Profile;
using LAMA.Core.Messages;

namespace LAMA.Auth;

public partial class UserSignUp : ContentPage
{
    private readonly FirebaseAuthClient _authClient;
    private readonly FirebaseAuthConfig fbConfig = new FirebaseAuthConfig
    {
        ApiKey = "AIzaSyDiAuutGePttuNIoUxGy2Ok6NDcqGoh74k",
        AuthDomain = "lama-60ddc.firebaseapp.com",
        Providers = new FirebaseAuthProvider[]
        {
        new EmailProvider()
        }
    };

    public UserSignUp()
    {
        InitializeComponent();

        _authClient = new FirebaseAuthClient(fbConfig);

    }

    private async void OnSignInTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//SignInPage");  // Navigate to SignInPage
    }

    private async void OnUserSignUpTapped(object sender, EventArgs e)
    {
        try
        {
            // Firebase auth
            var userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email_.Text, password_.Text);
            string uid = userCredential.User.Uid;
            string idToken = await userCredential.User.GetIdTokenAsync();

            // Construct model
            var newUser = new LAMA.Core.Profile.User
            {
                Uid = uid,
                EmailAddress = email_.Text,
                Username = username_.Text,
                FirstName = FirstName.Text,
                CreatedAt = DateTime.UtcNow,
                FrequentCategory = "",
                QueriesSubmitted = 0,
                ProfilePictureUrl = "",
                Conversations = new List<Conversation>()
            };

            // Serialize to Firestore format
            var storageServices = new UserStorageServices();
            string json = storageServices.ConvertUserToFirestoreJson(newUser);
            string url = $"https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/users?documentId={uid}&access_token={idToken}";

            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error storing user in Firestore: {response.StatusCode}\n\n{errorBody}");
            }

            await DisplayAlert("Success", "Account created successfully!", "OK");

            UserSession.CurrentUser = newUser;
            AppShell.Instance.ProfileContent.FlyoutItemIsVisible = true;

            await Shell.Current.GoToAsync("//ProfilePage");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred:\n{ex.Message}", "OK");
        }
    }
   
}