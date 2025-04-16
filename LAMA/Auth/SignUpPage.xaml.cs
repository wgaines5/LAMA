using LAMA.Core;
using System.Text;
using System.Text.Json;
using Firebase.Auth;
using Firebase.Auth.Providers;

namespace LAMA.Auth;

public partial class SignUpPage : ContentPage
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
    public SignUpPage()
    {
        InitializeComponent();

        _authClient = new FirebaseAuthClient(fbConfig);

    }

    private async void OnSignInTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//SignInPage");  // Navigate to SignInPage
    }

    private async void OnSignUpTapped(object sender, EventArgs e)
    {
        try
        {
            UserCredential mpCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(
                _email.Text, _password.Text);

            string uid = mpCredential.User.Uid;

            // ID token for authentication
            string idToken = await mpCredential.User.GetIdTokenAsync();

            // Usser fields for profile - Database
            object userData = new
            {
                email = _email.Text,
                username = _username.Text,
                firstName = FirstNameE.Text,
                lastName = LastNameE.Text,
                npi = NPIE.Text,
                state = StateE.Text,
                licenseNumber = LicNumber.Text,
                profileImageUrl = "usermock.png",
                isVerified = false,
                createdAt = DateTime.UtcNow.ToString("o")
            };

            string json = JsonSerializer.Serialize(userData);

            //  Firebase RT Database url 
            string url = $"https://lama-60ddc-default-rtdb.firebaseio.com/medical_providers/{uid}.json?auth={idToken}";

            
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Firebase Error: {response.StatusCode}\n\n{errorBody}");
            }

            //  Success msg shown to proceed 
            await DisplayAlert("Success", "Your Account Has Been created, Now Awaiting verification.", "OK");
            await Shell.Current.GoToAsync($"//{nameof(MPDashBoard)}");
        }
        catch 
        {
            await DisplayAlert("Error","Something Crashed Try Again", "OK");
        }
    }
}