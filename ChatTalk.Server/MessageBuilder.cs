using System;
using System.Collections.Generic;
using System.Text;

namespace ChatTalk.Server
{
    public static class MessageBuilder
    {
        private const string DELIMITER = "^||^";

        public static string CreateChatMessage(string userNm, string msgId, string msg)
        {
            return $"MSG{DELIMITER}{userNm}{DELIMITER}{msgId}{DELIMITER}{msg}";
        }

        public static string CreateUserListMessage(IEnumerable<string> users)
        {
            return $"USRLST{DELIMITER}{users}";
        }

        public static string CreateJoinMessage(string userNm)
        {
            return $"JOIN{DELIMITER}{userNm}";
        }

        public static string CreateLeaveMessage(string userNm)
        {
            return $"LEAVE{DELIMITER}{userNm}";
        }
    }
}
