using System.Text;
using System.Text.Json;
using Firebase.Auth;
using Firebase.Auth.Providers;
using LAMA.Core.Profile;



namespace LAMA.Auth;

public partial class UsrSignUp : ContentPage
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
    public UsrSignUp()
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
            UserCredential userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(
                email_.Text, password_.Text);

            string uid = userCredential.User.Uid;

            // ID token for authentication
            string idToken = await userCredential.User.GetIdTokenAsync();

            // Usser fields for profile - Database
            object userData = new
            {
                email = email_.Text,
                username = username_.Text,
                firstName = FirstName.Text,
                profileImageUrl = "userpic.png",
                createdAt = DateTime.UtcNow.ToString("o")
            };

            string json = JsonSerializer.Serialize(userData);

            //  Firebase RT Database url 
            string url = $"https://lama-60ddc-default-rtdb.firebaseio.com/anynomous_users/{uid}.json?auth={idToken}";


            HttpClient client = new HttpClient();
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Firebase Error: {response.StatusCode}\n\n{errorBody}");
            }

            //  Success msg shown to proceed 
            await DisplayAlert("Success", "Your Account Has Been created", "OK");
            await Shell.Current.GoToAsync($"//{nameof(ProfilePage)}");
        }
        catch
        {
            await DisplayAlert("Error", "Something Crashed Try Again", "OK");
        }
    }
}