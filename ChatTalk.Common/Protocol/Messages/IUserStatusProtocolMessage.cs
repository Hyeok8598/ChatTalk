using System;
using System.Collections.Generic;
using System.Text;

namespace ChatTalk.Common.Protocol.Messages
{
    public interface IUserStatusProtocolMessage
    {
        string UserName { get; }
        string StatusText { get; }
    }
}
