namespace AIChatModule.Models;

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

public class ChatSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1";
    public string DefaultModel { get; set; } = "openai/gpt-3.5-turbo";
}
