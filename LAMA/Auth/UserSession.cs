using Firebase.Auth;

using System.Text.Json;

namespace LAMA.Auth
{
    public static class UserSession
    {
        public static UserCredential Credential { get; set; }

        public static string UserId { get; set; }
        public static string Role { get; set; } = "guest";
        public static string Token { get; set; }
        public static LAMA.Core.Profile.User CurrentUser { get; set; }
        public static JsonElement CurrentMedicalProfessional { get; set; }

        public static bool IsMedPro => Role == "medpro_verified";
        public static bool IsUnverified => Role == "medpro_unverified";
        public static bool IsUser => Role == "user";
    }
}