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

            //Routing.RegisterRoute("auth/signup", typeof(LAMA.Auth.SignUpPage));

            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(MedFactsPage), typeof(MedFactsPage));
            Routing.RegisterRoute(nameof(CategoryPage), typeof(CategoryPage));
            Routing.RegisterRoute(nameof(MessagesPage), typeof(MessagesPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
        }
    }
}
