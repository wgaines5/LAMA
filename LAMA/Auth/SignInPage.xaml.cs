using System.Text.Json;
using Firebase.Auth;
using Firebase.Auth.Providers;
using LAMA.Core;
using LAMA.Core.Profile;

namespace LAMA.Auth;

public partial class SignInPage : ContentPage
{
    private readonly FirebaseAuthClient _authClient;

    public SignInPage()
    {
        InitializeComponent();

        _authClient = new FirebaseAuthClient(new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyDiAuutGePttuNIoUxGy2Ok6NDcqGoh74k",
            AuthDomain = "lama-60ddc.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        });
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        try
        {
            UserCredential credential = await _authClient.SignInWithEmailAndPasswordAsync(EmailEntry.Text, PasswordEntry.Text);
            UserSession.Credential = credential;
            string uid = credential.User.Uid;
            string idToken = await credential.User.GetIdTokenAsync();
            UserSession.Token = idToken;

            Preferences.Set("userEmail", EmailEntry.Text);
            Preferences.Set("userPassword", PasswordEntry.Text);

            bool isMedicalProfessional = await CheckIfDocumentExists("medical_providers", uid, idToken);
            bool isGeneralUser = await CheckIfDocumentExists("users", uid, idToken);

            if (isMedicalProfessional)
            {
                await LoadMedicalProfessionalIntoSession(uid, idToken);
                await Shell.Current.GoToAsync($"//{nameof(MPDashBoard)}");
            }
            else if (isGeneralUser)
            {
                await LoadUserIntoSession(uid, idToken);
                await Shell.Current.GoToAsync($"//{nameof(ProfilePage)}");
            }
            else
            {
                await DisplayAlert("Error", "No user record found in Firestore.", "OK");
            }
        }
        catch
        {
            await DisplayAlert("Error", "Login failed: Password or Username Invalid", "OK");
        }
    }

    private async Task<bool> CheckIfDocumentExists(string collection, string uid, string idToken)
    {
        string url = $"https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/{collection}/{uid}?access_token={idToken}";
        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(url);
        return response.IsSuccessStatusCode;
    }

    private async Task LoadUserIntoSession(string uid, string idToken)
    {
        string url = $"https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/users/{uid}?access_token={idToken}";
        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement fields = doc.RootElement.GetProperty("fields");

            LAMA.Core.Profile.User loadedUser = new LAMA.Core.Profile.User
            {
                Uid = uid,
                EmailAddress = fields.GetProperty("emailAddress").GetProperty("stringValue").GetString(),
                FirstName = fields.GetProperty("firstName").GetProperty("stringValue").GetString(),
                FrequentCategory = fields.GetProperty("frequentCategory").GetProperty("stringValue").GetString(),
                ProfilePictureUrl = fields.GetProperty("profilePictureUrl").GetProperty("stringValue").GetString(),
                QueriesSubmitted = int.Parse(fields.GetProperty("queriesSubmitted").GetProperty("integerValue").GetString()),
                CreatedAt = DateTime.Parse(fields.GetProperty("createdAt").GetProperty("timestampValue").GetString())
            };

            UserSession.CurrentUser = loadedUser;
        }
    }

    private async Task LoadMedicalProfessionalIntoSession(string uid, string idToken)
    {
        string url = $"https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/medical_providers/{uid}?access_token={idToken}";
        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            JsonDocument doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("fields", out JsonElement fields))
            {
                UserSession.CurrentMedicalProfessional = fields; 
            }
        }
    }

    private async void OnSignUpTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}