namespace ChatTalk.Common.Protocol.Messages
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