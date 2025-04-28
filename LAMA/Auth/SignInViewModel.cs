using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;

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
        await   _authClient.SignInWithEmailAndPasswordAsync(Email, Password);
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
            UserSession.Token = await uCredential.User.GetIdTokenAsync();
        }
    }
}
