using Microsoft.Extensions.Logging;
using Firebase.Auth;
using Firebase.Auth.Providers;
using LAMA.Auth;
using Firebase.Database;

namespace LAMA;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig()
		{
			ApiKey = "AIzaSyDiAuutGePttuNIoUxGy2Ok6NDcqGoh74k",
			AuthDomain = "lama-60ddc.firebaseapp.com",
			Providers = new FirebaseAuthProvider[]
			{
				new EmailProvider()
			}
        }));

		builder.Services.AddSingleton(new FirebaseClient("https://lama-7054a-default-rtdb.firebaseio.com/"));

        builder.Services.AddSingleton<AuthTestSignInPage>();
        builder.Services.AddSingleton<SignInViewModel>();
        builder.Services.AddSingleton<AuthTestSignUpPage>();
        builder.Services.AddSingleton<SignUpViewModel>();

        return builder.Build();
	}
}
