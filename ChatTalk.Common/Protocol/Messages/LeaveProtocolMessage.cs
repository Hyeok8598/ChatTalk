namespace ChatTalk.Common.Protocol.Messages
{
    /// <summary>
    /// 사용자 퇴장을 나타내는 프로토콜 클래스
    /// </summary>
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
