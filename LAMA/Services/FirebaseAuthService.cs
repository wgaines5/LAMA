using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth.Providers;

namespace LAMA.Services
{
    public class FirebaseAuthService
    {
        private readonly FirebaseAuthClient _firebaseAuthClient;

        public FirebaseAuthService()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = "",
                AuthDomain = "",
                // Providers = new FirebaseAuthProvider[]
                
            };
        }

        public async Task SignInAnonymously()
        {
            
        }
    }
}
