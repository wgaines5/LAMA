using LAMA.Core;
using LAMA.Auth;
using System.Collections.ObjectModel;

namespace LAMA
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

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ProfilePage));
        }

        private async void OnMedFactsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(MedFactsPage));
        }

        private async void OnCategoryClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CategoryPage));
        }

        private async void OnMessagesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(MessagesPage));
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(SettingsPage));
        }

        private async void OnMPSignupClicked(object sender, EventArgs e) 
        {
            await Shell.Current.GoToAsync(nameof(SignUpPage));
        }
    }

}
