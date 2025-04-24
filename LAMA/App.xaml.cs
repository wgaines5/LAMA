using Firebase.Auth.Providers;
using Firebase.Auth;
using LAMA.Auth;
using LAMA.Core;

namespace LAMA;

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

        _ = SignInIfSavedAsync();
    }

    private async Task SignInIfSavedAsync()
    {
        string email = Preferences.Get("userEmail", string.Empty);
        string password = Preferences.Get("userPassword", string.Empty);

        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            try
            {
                FirebaseAuthClient authClient = new FirebaseAuthClient(new FirebaseAuthConfig
                {
                    ApiKey = "AIzaSyDiAuutGePttuNIoUxGy2Ok6NDcqGoh74k",
                    AuthDomain = "lama-60ddc.firebaseapp.com",
                    Providers = new FirebaseAuthProvider[]
                    {
                    new EmailProvider()
                    }
                });

                UserCredential user = await authClient.SignInWithEmailAndPasswordAsync(email, password);
                UserSession.Credential = user;

                await Shell.Current.GoToAsync($"//{nameof(MPDashBoard)}");
            }
            catch
            {
                // Clear saved credentials if sign-in fails
                Preferences.Remove("userEmail");
                Preferences.Remove("userPassword");
            }
        }
    }

}


