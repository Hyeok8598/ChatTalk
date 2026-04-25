using System;
using System.Collections.Generic;
using System.Text;

namespace ChatTalk.Common.Protocol.Messages
{
    /// <summary>
    /// 사용자 상태 변화(입장, 퇴장 등)를 나타내는 프로토콜 인터페이스
    /// </summary>
    public interface IUserStatusProtocolMessage
    {
        string UserName { get; }
        string StatusText { get; }
    }
}
