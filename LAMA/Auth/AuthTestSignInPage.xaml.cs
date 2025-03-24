namespace LAMA.Auth;

public partial class AuthTestSignInPage : ContentPage
{
	public AuthTestSignInPage(SignInViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}