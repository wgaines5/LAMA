using LAMA.Core;
using LAMA.Auth;

using Microsoft.Maui.Controls;
namespace LAMA
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
        
            InitializeComponent();
         
            Routing.RegisterRoute(nameof(MedFactsPage), typeof(MedFactsPage));
            Routing.RegisterRoute(nameof(CategoryPage), typeof(CategoryPage));
            Routing.RegisterRoute(nameof(MessagesPage), typeof(MessagesPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
            Routing.RegisterRoute(nameof(MPDashBoard), typeof(MPDashBoard));

            Routing.RegisterRoute(nameof(GeneralHPage), typeof(GeneralHPage));
            Routing.RegisterRoute(nameof(MentalHPage), typeof(MentalHPage));
            Routing.RegisterRoute(nameof(SexualRPage), typeof(SexualRPage));
            Routing.RegisterRoute(nameof(ChronicAPage), typeof(ChronicAPage));
            Routing.RegisterRoute(nameof(MedicationDPage), typeof(MedicationDPage));
            Routing.RegisterRoute(nameof(AlternativeHPage), typeof(AlternativeHPage));
        }
    }
}
