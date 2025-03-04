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

        private async void OnCategorySelected(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string categoryName)
            {
                switch (categoryName)
                {
                    case "General Health":
                        await Shell.Current.GoToAsync(nameof(GeneralHPage));
                        break;
                    case "Mental Health":
                        await Shell.Current.GoToAsync(nameof(MentalHPage));
                        break;
                    case "Sexual & Reproductive Health":
                        await Shell.Current.GoToAsync(nameof(SexualRPage));
                        break;
                    case "Chronic Conditions & Autoimmune":
                        await Shell.Current.GoToAsync(nameof(ChronicAPage));
                        break;
                    case "Medication & Drug Interactions":
                        await Shell.Current.GoToAsync(nameof(MedicationDPage));
                        break;
                    case "Alternative & Holistic Medicine":
                        await Shell.Current.GoToAsync(nameof(AlternativeHPage));
                        break;
                }
            }
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