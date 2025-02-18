namespace LAMA
{
    public partial class MainPage : ContentPage
    {
        public double ScreenWidth => DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        public double ScreenHeight => DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        
        public double AppIconSize { get; set; }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            AppIconSize = ScreenWidth * .3;


        }

      
    }

}
