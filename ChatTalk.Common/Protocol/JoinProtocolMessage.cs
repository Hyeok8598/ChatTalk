using System;
using System.Collections.Generic;
using System.Text;

namespace ChatTalk.Common.Protocol
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
