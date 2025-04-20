using LAMA.Core;
using LAMA.Auth;
using LAMA.Core.Messages;
using LAMA.Core.Categories;
using LAMA.Core.Profile;


namespace LAMA
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
        
            InitializeComponent();
         
            Routing.RegisterRoute(nameof(MedFactsPage), typeof(MedFactsPage));
            Routing.RegisterRoute(nameof(CategoryPage), typeof(CategoryPage));
            Routing.RegisterRoute(nameof(InboxPage), typeof(InboxPage));
            Routing.RegisterRoute(nameof(MessagePage), typeof(MessagePage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
            Routing.RegisterRoute(nameof(UserSignUp), typeof(UserSignUp));
            Routing.RegisterRoute(nameof(MPDashBoard), typeof(MPDashBoard));
            Routing.RegisterRoute(nameof(About), typeof(About));

            Routing.RegisterRoute(nameof(GeneralHPage), typeof(GeneralHPage));
            Routing.RegisterRoute(nameof(MentalHPage), typeof(MentalHPage));
            Routing.RegisterRoute(nameof(SexualRPage), typeof(SexualRPage));
            Routing.RegisterRoute(nameof(ChronicAPage), typeof(ChronicAPage));
            Routing.RegisterRoute(nameof(MedicationDPage), typeof(MedicationDPage));
            Routing.RegisterRoute(nameof(AlternativeHPage), typeof(AlternativeHPage));


            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        }
    }
}
