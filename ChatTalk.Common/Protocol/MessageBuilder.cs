using System;
using System.Collections.Generic;
using System.Text;

namespace ChatTalk.Common.Protocol
{
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