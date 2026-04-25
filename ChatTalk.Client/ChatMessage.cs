using ChatTalk.Client.Model;

namespace ChatTalk.Client
{
    public class ChatMessage
    {
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public MessageDirection Direction { get; set; }

        public bool IsWhisper => Type == MessageType.Whisper;
    }
}