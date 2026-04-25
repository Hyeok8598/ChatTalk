using ChatTalk.Client.Model;

namespace ChatTalk.Client.Command
{
    /// <summary>
    /// 사용자 입력을 기반으로 생성되는 채팅 명령 객체
    /// </summary>
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