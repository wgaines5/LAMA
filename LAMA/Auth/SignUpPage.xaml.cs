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
            UserCredential mpCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(_email.Text, _password.Text);
            string uid = mpCredential.User.Uid;
            string idToken = await mpCredential.User.GetIdTokenAsync();

            object profile = new
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

            string json = ConvertToFirestoreJson(profile);
            string url = $"https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/medical_providers?documentId={uid}&access_token={idToken}";

            using HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Firestore Error: {response.StatusCode}\n\n{error}");
            }

            await DisplayAlert("Success", "Your Account Has Been created, Now Awaiting verification.", "OK");
            await Shell.Current.GoToAsync($"//{nameof(MPDashBoard)}");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Something Crashed\n{ex.Message}", "OK");
        }
    }

    private string ConvertToFirestoreJson(object data)
    {
        JsonElement element = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(data));
        Dictionary<string, object> fields = new();

        foreach (JsonProperty property in element.EnumerateObject())
        {
            object value;
            switch (property.Value.ValueKind)
            {
                case JsonValueKind.String:
                    value = new { stringValue = property.Value.GetString() };
                    break;
                case JsonValueKind.Number:
                    value = new { integerValue = property.Value.GetInt32().ToString() };
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    value = new { booleanValue = property.Value.GetBoolean() };
                    break;
                default:
                    value = new { stringValue = property.Value.ToString() };
                    break;
            }

            fields[property.Name] = value;
        }

        return JsonSerializer.Serialize(new { fields });
    }
}