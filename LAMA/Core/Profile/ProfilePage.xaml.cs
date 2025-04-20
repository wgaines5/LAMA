using Firebase.Auth;
using LAMA.Auth;

namespace LAMA.Core.Profile;

public partial class ProfilePage : ContentPage
{

    public ProfilePage()
    {
        InitializeComponent();
        var user = UserSession.CurrentUser;
        if (user != null)
        {
            firstNameLabel.Text = $"Hello, {user.FirstName}";
            createdAtLabel.Text = $"Joined {user.CreatedAt.ToString("MMMM d, yyyy")}";
        }
    
    }

    private void OnProfileImageClicked(object sender, EventArgs e)
    {
        Console.WriteLine("ImageButton clicked!");
    }





}