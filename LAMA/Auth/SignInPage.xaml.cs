using System.Collections.ObjectModel;
namespace LAMA.Auth;

public partial class SignInPage : ContentPage
{
    public double AppIconSize { get; set; }

    public SignInPage()
	{
        InitializeComponent();
	}

    private async void OnSignUpTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//SignUpPage");  // Navigate to SignUpPage
    }

    private async void OnSignInTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ProfilePage");  // Navigate to SignUpPage
    }

}