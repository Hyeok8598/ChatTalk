using System;
using System.Collections.Generic;
using System.Text;

namespace ChatTalk.Common.Protocol
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
