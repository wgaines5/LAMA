using Firebase.Auth;
using LAMA.Auth;
using Microsoft.Maui.Controls;


namespace LAMA.Core.Profile;

public partial class ProfilePage : ContentPage
{

    public ProfilePage()
    {
        InitializeComponent();
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

        // Bind to the CollectionView
        userMedFactsCollectionView.ItemsSource = facts;

    }

    public void SetUserInfo()
    {
        var user = UserSession.CurrentUser;
        if (user != null)
        {
            firstNameLabel.Text = $"Hello, {user.FirstName}";
            createdAtLabel.Text = $"Joined {user.CreatedAt.ToString("MMMM d, yyyy")}";
        }
        if (!string.IsNullOrEmpty(UserSession.CurrentUser.ProfilePictureUrl))
        {
            byte[] imageBytes = Convert.FromBase64String(UserSession.CurrentUser.ProfilePictureUrl);
            ProfilePic.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        }

    }

    //private void OnProfileImageClicked(object sender, EventArgs e)
    //{
    //    Console.WriteLine("ImageButton clicked!");
    //}


}