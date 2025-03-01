using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace LAMA.Core
{

    public partial class CategoryPage : ContentPage
    {
        public ObservableCollection<Category> Categories { get; set; }
        public CategoryPage()
        {
            InitializeComponent();
            Categories = new ObservableCollection<Category>
        {
            new() { Name = "General Health", Icon = "general_health.png"},
            new() { Name = "Mental Health", Icon = "mental_health.png"},
            new() { Name = "Sexual & Reproductive Health", Icon = "sexualrep_health.png"},
            new() { Name = "Chronic Conditions & Autoimmune", Icon = "chronicauto.png"},
            new() { Name = "Medication & Drug Interactions", Icon = "meddrug.png"},
            new() { Name = "Alternative & Holistic Medicine", Icon = "holistic_meds.png"}

        };
            BindingContext = this;
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage());

        }
        private async void OnMedFactsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MedFactsPage());

        }
        private async void OnCategoryClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CategoryPage());

        }
        private async void OnMessagesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Messages.InboxPage());

        }
        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());

        }
    }

    public class Category
    {
        public required string Name { get; set; }
        public required string Icon { get; set; }
    }
}