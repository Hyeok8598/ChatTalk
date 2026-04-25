using ChatTalk.Client.Command;
using ChatTalk.Client.Model;

namespace ChatTalk.Client.Commads
{
    /// <summary>
    /// 사용자 입력 문자열을 ChatCommand 객체로 변환하는 파서 클래스
    /// </summary>
    public static class ChatCommandParser
    {
        public static ChatCommand Parse(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new Exception("메시지가 비어 있습니다.");

            if(command.StartsWith("/w "))
            {
                string[] parts = command.Split(" ", 3);

                if (parts.Length < 3)
                    throw new Exception("귓속말 형식이 올바르지 않습니다. (/w 대상유저 메시지)");

                return new ChatCommand(
                    MessageType.Whisper,
                    parts[2],
                    parts[1]
                );
            }

            return new ChatCommand(
                MessageType.Normal,
                command
            );
        }
    }
}
