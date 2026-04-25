namespace ChatTalk.Common.Protocol.Messages
{
    public class UsrListProtocolMessage : ProtocolMessage
    {
        public string[] UserListContent { get; } = Array.Empty<string>();

        public UsrListProtocolMessage(string[] userListContent) : base("USRLIST")
        {
            UserListContent = userListContent;
        }
    }
}
