using System.Collections.Generic;

namespace AIChatModule.Models
{
    public class ChatMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class ChatRequest
    {
        public List<ChatMessage> Messages { get; set; } = new();
    }
}
