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
        }

        private async void OnUserProfileClicked(object sender, EventArgs e)
        {
            var animationTask = this.TranslateTo(-500, 0, 300, Easing.SinInOut);
            var navigationDelay = Task.Delay(500); // slight overlap

            await Task.WhenAll(animationTask, navigationDelay);
            await Shell.Current.GoToAsync("//ProfilePage"); // Navigate to ProfilePage
        }

        private async void OnEditPreferredDoctorsClicked(object sender, EventArgs e)
        {
            var animationTask = this.TranslateTo(-500, 0, 300, Easing.SinInOut);
            var navigationDelay = Task.Delay(500); // slight overlap

            await Task.WhenAll(animationTask, navigationDelay);
            await Shell.Current.GoToAsync("//PreferredDoctorsPage");
        }


        private async void OnAccessibilityClicked(object sender, EventArgs e)
        {
            var animationTask = this.TranslateTo(-500, 0, 300, Easing.SinInOut);
            var navigationDelay = Task.Delay(500); // slight overlap

            await Task.WhenAll(animationTask, navigationDelay);
            await Shell.Current.GoToAsync("//AccessibilityPage");
        }

        private async void OnSupportClicked(object sender, EventArgs e)
        {
            var animationTask = this.TranslateTo(-500, 0, 300, Easing.SinInOut);
            var navigationDelay = Task.Delay(500); // slight overlap

            await Task.WhenAll(animationTask, navigationDelay);
            await Shell.Current.GoToAsync("//SupportPage"); // Navigate to SupportPage
        }

        private async void OnTOSClicked(object sender, EventArgs e)
        {
            var animationTask = this.TranslateTo(-500, 0, 300, Easing.SinInOut);
            var navigationDelay = Task.Delay(500); // slight overlap

            await Task.WhenAll(animationTask, navigationDelay);
            await Shell.Current.GoToAsync("//TOSPage"); // Navigate to TOSPage
        }

        private async void OnChangeAccountsClicked(object sender, EventArgs e)
        {
            var animationTask = this.TranslateTo(-500, 0, 300, Easing.SinInOut);
            var navigationDelay = Task.Delay(500); // slight overlap

            await Task.WhenAll(animationTask, navigationDelay);
            await Shell.Current.GoToAsync("//UsrSignUp");
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            var animationTask = this.TranslateTo(-500, 0, 300, Easing.SinInOut);
            var navigationDelay = Task.Delay(500); // slight overlap

            await Task.WhenAll(animationTask, navigationDelay);
            await Shell.Current.GoToAsync("//UsrSignUp");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Avoid double-animation when navigating back
            if (TranslationX == 0)
                this.TranslationX = 500;

            await this.TranslateTo(0, 0, 300, Easing.SinInOut);
        }
    }
}