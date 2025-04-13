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
            //var slide = this.TranslateTo(-500, 0, 500, Easing.SinInOut);
            //var fade = this.FadeTo(0, 500, Easing.SinInOut);                   /Option 2 if animation is favored

            //await Task.WhenAll(slide, fade);
            await Shell.Current.GoToAsync("//AccessibilityPage");

            //this.Opacity = 1;
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