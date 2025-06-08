using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LAMA.Core.Messages.MessagePage;
using LAMA.Core.Profile;

namespace LAMA.Auth
{
    public static class AuthServices
    {

        public async static Task<User?> SignInAnonymouslyAsync()
        {
            const string apiKey = "AIzaSyDiAuutGePttuNIoUxGy2Ok6NDcqGoh74k";
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}";
            var payload = new
            {
                returnSecureToken = true
            };

            var httpClient = new HttpClient();
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(payload),
                Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<FirebaseAuthResponse>(responseString);

            var anonUser = new User
            {
                Uid = result.localId,
                Username = "Anonymous",
                CreatedAt = DateTime.UtcNow,
                IsAnonymous = true
            };

            return anonUser;
        }
   
    

    public static void AuthorizedFlyoutMenuConfig()
        {
            AppShell.Instance.ProfileContent.FlyoutItemIsVisible = true;
            AppShell.Instance.MPDashboardContent.FlyoutItemIsVisible = true;
            AppShell.Instance.MPInboxContent.FlyoutItemIsVisible = true;

        }

    }
}
public class FirebaseAuthResponse
{
    public string idToken { get; set; }
    public string localId { get; set; }
    public string refreshToken { get; set; }
    public string expiresIn { get; set; }
}


