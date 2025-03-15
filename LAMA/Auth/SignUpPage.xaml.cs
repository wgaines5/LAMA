using LAMA.Core;

namespace LAMA.Auth;

public partial class SignUpPage : ContentPage
{
	public SignUpPage()
	{
		InitializeComponent();
	}

    private async void OnSignInTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//SignInPage");  // Navigate to SignInPage
    }

    private async void OnSignUpTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Congradulations", "You Are Now Signed Up and awainting Verification!", "Give It A Min");

        await Shell.Current.GoToAsync($"//{nameof(MPDashBoard)}");
    }
}