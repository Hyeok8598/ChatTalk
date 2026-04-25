namespace ChatTalk.Common.Protocol.Messages
{
    public class LeaveProtocolMessage : ProtocolMessage
    {
        public string UserName { get; }

        public LeaveProtocolMessage(string userName) : base("LEAVE")
        {
            UserName = userName;
        }
    }
}
