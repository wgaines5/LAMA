namespace LAMA.Core;

public partial class ProfilePage : ContentPage
{

	private string fullName;

	public ProfilePage()
	{
		InitializeComponent();
        //AnimatePageEntry();
	}

    private async void AnimatePageEntry()
    {
        this.TranslationX = 500; // Start off-screen (right side)
        await this.TranslateTo(0, 0, 300, Easing.SinInOut); // Slide in smoothly
    }
}