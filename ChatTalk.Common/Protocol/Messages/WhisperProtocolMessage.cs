namespace ChatTalk.Common.Protocol.Messages
{
    /// <summary>
    /// 귓속말 메시지를 나타내는 프로토콜 클래스
    /// </summary>
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