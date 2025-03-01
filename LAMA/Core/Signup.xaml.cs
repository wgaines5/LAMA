using System.Collections.ObjectModel;
namespace LAMA.Core;

public partial class Signup : ContentPage
{
    public double AppIconSize { get; set; }

    public Signup()
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