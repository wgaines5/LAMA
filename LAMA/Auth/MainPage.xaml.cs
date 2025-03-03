using LAMA.Auth;
using System.Collections.ObjectModel;

namespace LAMA.Auth
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<string> Categories { get; set; }

        public MainPage()
        {
            InitializeComponent();

            Categories = new ObservableCollection<string>
            {
                "General Health",
                "Mental Health",
                "Sexual Health",
                "Chronic Conditions & Autoimmune",
                "Medication & Drug Interactions",
                "Alternative & Holistic Medicine"
            };
            BindingContext = this;
        }


        private async void OnMPSignupClicked(object sender, EventArgs e) 
        {
            await Shell.Current.GoToAsync("//SignUpPage");
        }
    }

}
