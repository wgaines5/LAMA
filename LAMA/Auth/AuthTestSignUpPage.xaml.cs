namespace LAMA.Auth;

public partial class AuthTestSignUpPage : ContentPage
{
	public AuthTestSignUpPage(SignUpViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}