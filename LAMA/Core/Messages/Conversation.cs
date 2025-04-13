using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAMA.Core.Messages
{
    public class Conversation
    {
        public string ConversationId { get; set; }
        public List<string> ParticipantIds { get; set; }
        public List<Message> Messages { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
