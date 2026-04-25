namespace ChatTalk.Common.Protocol.Building
{
    /// <summary>
    /// 프로토콜 메시지를 생성하는 빌더 클래스
    /// 서버와 클라이언트 간 통신에 사용되는 문자열을 생성한다.
    /// </summary>
    public static class MessageBuilder
    {
        private const string DELIMITER = "^||^";

        public static string CreateChatMessage(string userNm, string msgId, string msg)
        {
            return $"MSG{DELIMITER}{userNm}{DELIMITER}{msgId}{DELIMITER}{msg}\n";
        }

        public static string CreateUsrListMessage(IEnumerable<string> users)
        {
            string userList = string.Join(",", users);
            return $"USRLIST{DELIMITER}{userList}\n";
        }

        public static string CreateJoinMessage(string userNm)
        {
            return $"JOIN{DELIMITER}{userNm}\n";
        }

        public static string CreateLeaveMessage(string userNm)
        {
            return $"LEAVE{DELIMITER}{userNm}\n";
        }

        public static string CreateWhisperMessage(string fromUsrNm, string toUsrNm, string msg)
        {
            return $"WHISPER{DELIMITER}{fromUsrNm}{DELIMITER}{toUsrNm}{DELIMITER}{msg}\n";
        }
    }
}