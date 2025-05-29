using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAMA.Core.Messages
{
    public class Message
    {
        public string SenderId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; } = false;
        public string profilePictureUrl { get; set; }

        public Message () { }
    }
}
