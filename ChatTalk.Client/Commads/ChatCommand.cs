using ChatTalk.Client.Model;

namespace ChatTalk.Client.Command
{
    public class ChatCommand
    {
        public MessageType Type { get; }
        public string? ToUserName { get; }
        public string Content { get; } = string.Empty;

        public ChatCommand(MessageType type, string content, string? toUserName = null)
        {
            Type = type;
            Content = content;
            ToUserName = toUserName;
        }

        public ChatMessage ToChatMessage(string senderName)
        {
            return new ChatMessage
            {
                SenderName = senderName,
                Content = Content,
                Type = this.Type,
                Direction = MessageDirection.Sent
            };
        }
    }
}
