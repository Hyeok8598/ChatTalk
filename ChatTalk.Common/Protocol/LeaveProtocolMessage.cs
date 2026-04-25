using System;
using System.Collections.Generic;
using System.Text;

namespace ChatTalk.Common.Protocol
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
