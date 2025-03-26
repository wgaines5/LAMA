using LAMA.Core;
using CommunityToolkit.Mvvm;
using Firebase.Auth;
using Google.Cloud.Firestore;


namespace LAMA.Auth;

public partial class SignUpPage : ContentPage
{
    private readonly FirebaseAuthClient _authClient;
    private readonly FirestoreDb firestoreDb;
    private readonly FirebaseAuthConfig fbConfig = new FirebaseAuthConfig
    {
        ApiKey = "AIzaSyDiAuutGePttuNIoUxGy2Ok6NDcqGoh74k",
        AuthDomain = "LAMA.firebaseapp.com",
    }; 
	public SignUpPage()
	{
		InitializeComponent();

        _authClient = new FirebaseAuthClient(fbConfig);
        firestoreDb = FirestoreDb.Create("lama-60ddc");
	}

    private async void OnSignInTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//SignInPage");  // Navigate to SignInPage
    }

    private async void OnSignUpTapped(object sender, EventArgs e)
    {
        try
        {
            // 1. Create user with email & password
            UserCredential MPCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(
                _email.Text, _password.Text);

            string MpId = MPCredential.User.Uid;

            // 2. Build Firestore document data
            Dictionary<string, object> userProfile = new Dictionary<string, object>
            {
                { "Email", _email.Text },
                { "Username", _username.Text },
                { "FirstName", FirstNameE.Text },
                { "LastName", LastNameE.Text },
                { "NPI", NPIE.Text },
                { "State", StateE.Text },
                { "LicenseNumber", LicNumber.Text },
                { "ProfileImageUrl", "usermock.png" }, 
                { "IsVerified", false },
                { "CreatedAt", Timestamp.GetCurrentTimestamp() }
            };

            // 3. Save to Firestore
            DocumentReference docRef = firestoreDb.Collection("medical_providers").Document(MpId);
            await docRef.SetAsync(userProfile);

            await DisplayAlert("Success", "Your Medical Professional Account Has Been Created. Awaiting verification.", "OK");
            await Shell.Current.GoToAsync($"//{nameof(MPDashBoard)}");
        }
        catch 
        {
            await DisplayAlert("Error", "Account Not Created", "OK");
        }
    }
}