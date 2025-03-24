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
    }
}
