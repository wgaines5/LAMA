using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace LAMA.Core
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            //AnimatePageEntry();
        }

        private async void OnUserProfileClicked(object sender, EventArgs e)
        {
            //await this.TranslateTo(-500, 0, 300, Easing.SinInOut); // Slide left before navigating
            await Shell.Current.GoToAsync("//ProfilePage"); // Navigate to ProfilePage
        }

        private async void OnEditPreferredDoctorsC(object sender, EventArgs e)
        {
            //await this.TranslateTo(-500, 0, 300, Easing.SinInOut); // Slide left before navigating
            await Shell.Current.GoToAsync("//PreferredDoctorsPage"); // Navigate to PreferredDoctorsPage
        }

        private async void OnAccessibilityClicked(object sender, EventArgs e)
        {
            //await this.TranslateTo(-500, 0, 300, Easing.SinInOut); // Slide left before navigating
            //await Shell.Current.GoToAsync("//AccessibilityPage"); // Navigate to AccessibilityPage
        }

        private async void OnSupportClicked(object sender, EventArgs e)
        {
            //await this.TranslateTo(-500, 0, 300, Easing.SinInOut); // Slide left before navigating
            //await Shell.Current.GoToAsync("//SupportPage"); // Navigate to SupportPage
        }

        private async void OnTOSClicked(object sender, EventArgs e)
        {
            //await this.TranslateTo(-500, 0, 300, Easing.SinInOut); // Slide left before navigating
            //await Shell.Current.GoToAsync("//TOSPage"); // Navigate to TOSPage
        }

        private async void OnChangeAccountsClicked(object sender, EventArgs e)
        {
            //await this.TranslateTo(-500, 0, 300, Easing.SinInOut); // Slide left before navigating
            //await Shell.Current.GoToAsync("//SignInPage"); // Navigate to SignInPage
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            //await this.TranslateTo(-500, 0, 300, Easing.SinInOut); // Slide left before navigating
            //await Shell.Current.GoToAsync("//Logout"); // Navigate to LogoutPage
        }

        private async void AnimatePageEntry()
        {
            this.TranslationX = 500; // Start off-screen (right side)
            await this.TranslateTo(0, 0, 300, Easing.SinInOut); // Slide in smoothly
        }
    }
}