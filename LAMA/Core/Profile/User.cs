using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Cloud.Firestore;
using LAMA.Core.Messages;

namespace LAMA.Core.Profile
{ 
  
    public class User
    {
        public string Uid { get; set; }
        public string EmailAddress { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public int QueriesSubmitted { get; set; } = 0;
        public string FrequentCategory { get; set; } = "";
        public string ProfilePictureUrl { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public List<string> BookmarkedMedFacts { get; set; } = new List<string>();

        public User() { }
    }
}

