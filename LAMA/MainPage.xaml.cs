namespace LAMA
{
    public partial class MainPage : ContentPage
    {

        public double AppIconSize { get; set; }

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSignUpTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignUpPage");  // Navigate to SignUpPage
        }

        private async void OnSignInTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");  // Navigate to ProfilePage
        }

    }

}
