namespace LAMA.Core.Messages
{
    public class MessageItem
    {
        public string SenderId { get; set; }
        public string Message { get; set; }
        public string Timestamp { get; set; }
        public bool IsAssigned { get; set; }
        public string ProfilePic { get; set; }
        public string SessionId { get; set; }
        public string Category { get; set; }
    }
}