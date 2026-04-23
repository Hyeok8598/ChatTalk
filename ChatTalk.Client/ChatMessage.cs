using ChatTalk.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatTalk.Client
{
    public class ChatMessage
    {
        public string SenderName { get; set; }
        public string Content { get; set; }
        public MessageType Type { get; set; }
        public MessageDirection Direction { get; set; }

        public bool IsWhisper => Type == MessageType.Whisper;
    }
}