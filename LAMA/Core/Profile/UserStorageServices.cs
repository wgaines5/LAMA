using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LAMA.Core.Profile
{
    public class UserStorageServices
    {

        public UserStorageServices() { }
      
        public string ConvertUserToFirestoreJson(LAMA.Core.Profile.User user)
        {
            var firestoreJson = new
            {
                fields = new
                {
                    uid = new { stringValue = user.Uid },
                    emailAddress = new { stringValue = user.EmailAddress },
                    firstName = new { stringValue = user.FirstName },
                    createdAt = new { timestampValue = user.CreatedAt.ToString("o") },
                    queriesSubmitted = new { integerValue = user.QueriesSubmitted.ToString() },
                    frequentCategory = new { stringValue = user.FrequentCategory },
                    profilePictureUrl = new { stringValue = user.ProfilePictureUrl },
                    conversations = new
                    {
                        arrayValue = new { values = new object[] { } }
                    },

                      bookmarkedMedFacts = new
                      {
                          arrayValue = new { values = new object[] { } }
                      }
                }
            };

            return JsonSerializer.Serialize(firestoreJson);
        }

        public void IncrementUserQueryCount(LAMA.Core.Profile.User user)
        {

        }
    }
}
