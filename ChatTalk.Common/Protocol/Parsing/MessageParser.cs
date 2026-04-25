using ChatTalk.Common.Protocol.Messages;

namespace ChatTalk.Common.Protocol.Parsing
{
    /// <summary>
    /// 서버와 클라이언트 간에 송수신되는 프로토콜 문자열을
    /// ProtocolMessage 객체로 변환하는 파서 클래스
    /// </summary>
    public static class MessageParser
    {
        private const string DELIMITER = "^||^";

        public static ProtocolMessage Parse(string raw)
        {
            string[] parts = raw.Split(DELIMITER);

            if (parts.Length == 0)
                throw new Exception("Invaild message.");

            string type = parts[0];

            return type switch
            {
                "MSG" => ParseChat(parts),
                "WHISPER" => ParseWhisper(parts),
                "USRLIST" => ParseUsrList(parts),
                "JOIN" => ParseJoin(parts),
                "LEAVE" => ParseLeave(parts),

                _ => throw new Exception($"Unknown message type: {type}")
            };
        }

        private static ChatProtocolMessage ParseChat(string[] parts)
        {
            if (parts.Length < 4)
                throw new Exception("Invaild MSG message");

            string sender = parts[1];
            string messageId = parts[2];
            string content = string.Join(DELIMITER, parts.Skip(3));
            
            return new ChatProtocolMessage(sender, messageId, content);
        }

        private static WhisperProtocolMessage ParseWhisper(string[] parts)
        {
            if (parts.Length < 4)
                throw new Exception("Invaild WHISPER message");

            string sender = parts[1];
            string receiver = parts[2];
            string content = string.Join(DELIMITER, parts.Skip(3));

            return new WhisperProtocolMessage(sender, receiver, content);
        }

        private static UsrListProtocolMessage ParseUsrList(string[] parts)
        {
            if (parts.Length < 2)
                throw new Exception("Invaild USRLIST message");

            string[] userList = parts[1].Split(",");

            return new UsrListProtocolMessage(userList);
        }

        private static JoinProtocolMessage ParseJoin(string[] parts)
        {
            if (parts.Length < 2)
                throw new Exception("Invail JOIN message");

            string userNm = parts[1];

            return new JoinProtocolMessage(userNm);
        }

        private static LeaveProtocolMessage ParseLeave(string[] parts)
        {
            if (parts.Length < 2)
                throw new Exception("Invail LEAVE message");

            string userNm = parts[1];

            return new LeaveProtocolMessage(userNm);
        }
    }
}
