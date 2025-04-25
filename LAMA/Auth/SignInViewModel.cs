using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Google.Cloud.Firestore;
using LAMA.Core;

namespace LAMA.Auth
{
    public partial class SignInViewModel : ObservableObject
    {
        private readonly FirebaseAuthClient _authClient;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        public SignInViewModel(FirebaseAuthClient authClient)
        {
            _authClient = authClient;
        }

        [RelayCommand]
        private async Task SignIn()
        {
            var credential = await _authClient.SignInWithEmailAndPasswordAsync(Email, Password);

            UserSession.Credential = credential;
            UserSession.UserId = credential.User.Uid;

            // Load user role from Firestore
            FirestoreDb db = FirestoreDb.Create("lama-60ddc");
            var doc = await db.Collection("users").Document(credential.User.Uid).GetSnapshotAsync();

            if (doc.Exists && doc.TryGetValue("role", out string role))
                UserSession.Role = role;
            else
                UserSession.Role = "guest";

            // Navigate page based on role
            if (UserSession.Role == "user")
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            else
                await Shell.Current.GoToAsync($"//{nameof(MPDashBoard)}");
        }

        [RelayCommand]
        private async Task NavigateSignUp()
        {
            await Shell.Current.GoToAsync("//TestSignUp");
        }

        [RelayCommand]
        public async Task UserActive()
        {
            string email = Preferences.Get("userEmail", string.Empty);
            string password = Preferences.Get("userPassword", string.Empty);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return;

            UserCredential uCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);
            UserSession.Credential = uCredential;
        }
    }
}
