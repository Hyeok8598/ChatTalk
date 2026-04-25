namespace ChatTalk.Common.Protocol.Messages
{
    public class LeaveProtocolMessage : ProtocolMessage, IUserStatusProtocolMessage
    {
        public string UserName { get; }
        public string StatusText => $"{UserName} 님이 나갔습니다.";

        public LeaveProtocolMessage(string userName) : base("LEAVE")
        {
            UserName = userName;
        }
    }
}
