namespace ChatTalk.Common.Protocol.Messages
{
    /// <summary>
    /// 일반 채팅 메시지를 나타내는 프로토콜 클래스
    /// </summary>
    public class ChatProtocolMessage : ProtocolMessage
    {
        public string SenderName { get; }
        public string MessageId { get; }
        public string Content { get; }

        public ChatProtocolMessage(string senderName, string messageId, string content) : base("MSG")
        {
            SenderName = senderName;
            MessageId = messageId;
            Content = content;
        }
    }
}
