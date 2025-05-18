using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LAMA.Core.Messages
{
    public class UnassignedMessage
    {
        public bool IsAssigned { get; set; }
        [JsonPropertyName("message")]
        public string Text { get; set; }
        public string SenderId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
