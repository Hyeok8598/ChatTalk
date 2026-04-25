namespace ChatTalk.Common.Protocol.Messages
{
    /// <summary>
    /// 모든 프로토콜 메시지의 기본 클래스
    /// </summary>
    public abstract class ProtocolMessage
    {
        public string Type { get; }

        protected ProtocolMessage(string type)
        {
            Type = type;
        }
    }
}