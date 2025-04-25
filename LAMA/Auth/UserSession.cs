using Firebase.Auth;

namespace LAMA.Auth
{
    public static class UserSession
    {
        public static UserCredential Credential { get; set; }

        public static string UserId { get; set; }
        public static string Role { get; set; } = "guest";

        public static bool IsMedPro => Role == "medpro_verified";
        public static bool IsUnverified => Role == "medpro_unverified";
        public static bool IsUser => Role == "user";
    }
}