namespace ChatTalk.Common.Protocol.Messages
{
    public class JoinProtocolMessage : ProtocolMessage
    {
        public string UserName { get; }

        public JoinProtocolMessage(string userName) : base("JOIN")
        {
            UserName = userName;
        }
    }
}
