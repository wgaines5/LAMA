using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using LAMA.Core.Categories;

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

        //Allows user to visit the specific cat page
        private async void OnCategorySelected(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string categoryName)
            {
                switch (categoryName)
                {
                    case "General Health":
                        await Shell.Current.GoToAsync("//MainPage");
                        await Shell.Current.GoToAsync($"{nameof(GeneralHPage)}");
                        break;
                    case "Mental Health":
                        await Shell.Current.GoToAsync("//MainPage");
                        await Shell.Current.GoToAsync(nameof(MentalHPage));
                        break;
                    case "Sexual & Reproductive Health":
                        await Shell.Current.GoToAsync("//MainPage");
                        await Shell.Current.GoToAsync(nameof(SexualRPage));
                        break;
                    case "Chronic Conditions & Autoimmune":
                        await Shell.Current.GoToAsync("//MainPage");
                        await Shell.Current.GoToAsync(nameof(ChronicAPage));
                        break;
                    case "Medication & Drug Interactions":
                        await Shell.Current.GoToAsync("//MainPage");
                        await Shell.Current.GoToAsync(nameof(MedicationDPage));
                        break;
                    case "Alternative & Holistic Medicine":
                        await Shell.Current.GoToAsync("//MainPage");
                        await Shell.Current.GoToAsync(nameof(AlternativeHPage));
                        break;
                }
            }
        }
    }

    public class Category
    {
        public required string Name { get; set; }
        public required string Icon { get; set; }
    }
}