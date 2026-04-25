namespace ChatTalk.Common.Protocol.Messages
{
    /// <summary>
    /// 현재 접속 사용자 목록을 나타내는 프로토콜 클래스
    /// </summary>
    public class UsrListProtocolMessage : ProtocolMessage
    {
        public string[] UserListContent { get; } = Array.Empty<string>();

        public UsrListProtocolMessage(string[] userListContent) : base("USRLIST")
        {
            UserListContent = userListContent;
        }
    }
}
