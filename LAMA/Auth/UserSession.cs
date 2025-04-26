using Firebase.Auth;

namespace LAMA.Auth
{
    public static class UserSession
    {
        public static UserCredential Credential { get; set; }

        public static LAMA.Core.Profile.User CurrentUser { get; set; }
        public static string Token { get; set; }
        public static string ProviderId { get; set; }
        public static string SessionId {  get; set; }
    }
}