using System;
using System.Collections.Generic;
using System.Text;

namespace ChatTalk.Common.Protocol
{
    public abstract class ProtocolMessage
    {
        public string Type { get; }

        protected ProtocolMessage(string type)
        {
            Type = type;
        }
    }
}