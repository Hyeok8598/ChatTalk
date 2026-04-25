namespace ChatTalk.Common.Protocol.Messages
{
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
