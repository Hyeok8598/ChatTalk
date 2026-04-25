namespace ChatTalk.Common.Protocol.Messages
{
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
