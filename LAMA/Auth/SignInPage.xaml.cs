namespace LAMA.Auth;

public partial class SignInPage : ContentPage
{
    public SignInPage()
    {
        InitializeComponent();
    }
          private async void OnSignUpTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");  // Navigate to SignUpPage
    }

}

