using Firebase.Auth;
using LAMA.Auth;


namespace LAMA.Core.Profile;

public partial class ProfilePage : ContentPage
{

    bool UserSignedIn = UserSession.CurrentUser != null;

    public ProfilePage()
    {
        InitializeComponent();
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