namespace ChatTalk.Common.Protocol.Messages
{
    /// <summary>
    /// 사용자 입장을 나타내는 프로토콜 클래스
    /// </summary>
    public class JoinProtocolMessage : ProtocolMessage, IUserStatusProtocolMessage
    {
        public string UserName { get; }
        public string StatusText => $"{UserName} 님이 입장하였습니다.";

        public JoinProtocolMessage(string userName) : base("JOIN")
        {
            UserName = userName;
        }
    }
}
