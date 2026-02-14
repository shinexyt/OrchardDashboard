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
        public string Model { get; set; } = "openai/gpt-3.5-turbo";
    }

    public class ChatStreamChunk
    {
        public string Content { get; set; } = string.Empty;
        public bool Done { get; set; }
    }
}
