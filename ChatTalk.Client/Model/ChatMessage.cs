namespace ChatTalk.Client.Model
{
    /// <summary>
    /// 채팅 UI에 표시하기 위한 메시지 모델
    /// 서버에서 수신한 프로토콜 메시지를 UI 형태로 가공한 데이터
    /// </summary>
    public class ChatMessage
    {
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public MessageDirection Direction { get; set; }

        public bool IsWhisper => Type == MessageType.Whisper;
    }
}