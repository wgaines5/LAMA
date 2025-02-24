namespace LAMA
{
    public partial class MainPage : ContentPage
    {

        public double AppIconSize { get; set; }

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSignInTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignInPage");  // Navigate to SignInPage
        }

    }

}
