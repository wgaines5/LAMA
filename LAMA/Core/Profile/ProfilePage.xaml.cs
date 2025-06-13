using Firebase.Auth;
using LAMA.Auth;
using Microsoft.Maui.Controls;
using System.IO;

namespace LAMA.Core.Profile
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();

            // ? Enforce session before anything else
            if (UserSession.Credential == null)
            {
                Shell.Current.GoToAsync("//SignInPage");
                return;
            }

            SetUserInfo();

            // Sample mock data
            var facts = new List<string>
            {
                "The human body contains 206 bones.",
                "Your heart beats around 100,000 times per day.",
                "Laughing is good for your cardiovascular health.",
                "The brain uses 20% of the body's oxygen.",
                "Skin is the body's largest organ."
            };

            userMedFactsCollectionView.ItemsSource = facts;
        }

        public void SetUserInfo()
        {
            var user = UserSession.CurrentUser;
            if (user == null) return;

            firstNameLabel.Text = $"Hello, {user.FirstName}";
            createdAtLabel.Text = $"Joined {user.CreatedAt:MMMM d, yyyy}";

            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(user.ProfilePictureUrl);
                    ProfilePic.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
                catch
                {
                    // Optional: show placeholder
                    ProfilePic.Source = "default_profile.png";
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (UserSession.Credential == null)
            {
                Shell.Current.GoToAsync("//SignInPage");
                return;
            }
        }

        //private void OnProfileImageClicked(object sender, EventArgs e)
        //{
        //    Console.WriteLine("ImageButton clicked!");
        //}
    }
}