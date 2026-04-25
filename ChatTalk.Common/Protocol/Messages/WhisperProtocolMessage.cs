namespace ChatTalk.Common.Protocol.Messages
{
    public class WhisperProtocolMessage : ProtocolMessage
    {
        public string FromUserName { get; }
        public string ToUserName { get; }
        public string Content { get; }

        public WhisperProtocolMessage(string fromUserName, string toUserName, string content) : base("WHISPER")
        {
            FromUserName = fromUserName;
            ToUserName = toUserName;
            Content = content;
        }
    }
}