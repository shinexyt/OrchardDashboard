namespace AIChatModule.Models
{
    public class OpenRouterSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1";
        public string Model { get; set; } = "openai/gpt-3.5-turbo";
    }
}
