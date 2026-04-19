using System;
using System.Collections.Generic;
using System.Text;

namespace ChatTalk.Server
{
    public static class MessageParser
    {
        private const string DELIMITER = "^||^";

        public static ParsedMessage Parse(string message)
        {
            ParsedMessage parsedMessage = new();
            string[] parts;

            if (string.IsNullOrEmpty(message))
                return parsedMessage;
            
            parts = message.Split(DELIMITER);

            if(parts.Length < 2)
                return parsedMessage;

            parsedMessage.Type = parts[0];
            
            for(int i = 1; i < parts.Length; i++)
                parsedMessage.Type = parts[i];

            return parsedMessage;
        }
    }
}
