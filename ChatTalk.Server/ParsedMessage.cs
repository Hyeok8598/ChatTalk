using System;
using System.Collections.Generic;
using System.Text;

namespace ChatTalk.Server
{
    public class ParsedMessage
    {
        public string Type { get; set; } = string.Empty;
        public List<string> Values { get; set; } = new();
    }
}
