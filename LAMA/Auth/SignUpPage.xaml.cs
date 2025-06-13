using LAMA.Core;
using System.Text;
using System.Text.Json;
using Firebase.Auth;
using Firebase.Auth.Providers;

namespace LAMA.Auth;

public partial class SignUpPage : ContentPage
{
    private FileResult _selectedImage;
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
    private string _base64Image = string.Empty;

    private async void OnImageTapped(object sender, EventArgs e)
    {
        string action = await DisplayActionSheet("Choose Photo Option", "Cancel", null, null, "Pick from Gallery");

        FileResult photo = null;

       if (action == "Pick from Gallery")
        {
            photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Select a Profile Picture"
            });
        }

        if (photo != null)
        {
            _selectedImage = photo;

            using Stream stream = await photo.OpenReadAsync();
            using MemoryStream ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            byte[] imageBytes = ms.ToArray();

            _base64Image = Convert.ToBase64String(imageBytes);
            ProfileUpload.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        }
        else
        {
            FileResult result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select a Profile Picture",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                _selectedImage = result;
                ProfileUpload.Source = ImageSource.FromFile(result.FullPath);
            }
        }
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

            string base64Image;

            if (!string.IsNullOrEmpty(_base64Image))
            {
                base64Image = _base64Image;
            }
            else
            {
                base64Image = await ConvertImageToBase64Async();
            }

            object profile = new
            {
                email = _email.Text,
                username = _username.Text,
                firstName = FirstNameE.Text,
                lastName = LastNameE.Text,
                npi = NPIE.Text,
                state = StateE.Text,
                licenseNumber = LicNumber.Text,
                ProfilePic = base64Image,
                isVerified = false,
                createdAt = DateTime.UtcNow.ToString("o"),
                messageCount = ""
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

            Preferences.Set("userEmail", _email.Text);
            Preferences.Set("userPassword", _password.Text);

            await DisplayAlert("Success", "Your Account Has Been created, Now Awaiting verification.", "OK");
            await Shell.Current.GoToAsync($"//{nameof(MPDashBoard)}");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Something Crashed\n{ex.Message}", "OK");
        }
    }

    private async Task<string> ConvertImageToBase64Async()
    {
        if (_selectedImage == null)
            return string.Empty;

        using Stream stream = await _selectedImage.OpenReadAsync();
        using MemoryStream ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        byte[] imageBytes = ms.ToArray();
        return Convert.ToBase64String(imageBytes);
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
