using AIChatModule.Models;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Runtime.CompilerServices;

namespace AIChatModule.Services;

public interface IChatService
{
    IAsyncEnumerable<string> StreamChatAsync(List<Models.ChatMessage> messages, string model, CancellationToken cancellationToken = default);
}

public class OpenRouterChatService : IChatService
{
    private readonly ChatSettings _settings;

    public OpenRouterChatService(IOptions<ChatSettings> settings)
    {
        _settings = settings.Value;
    }

    public async IAsyncEnumerable<string> StreamChatAsync(List<Models.ChatMessage> messages, string model, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var apiKey = _settings.ApiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            yield return "Error: API key not configured. Please set OPENROUTER_API_KEY in appsettings.json";
            yield break;
        }

        var openAIOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri(_settings.BaseUrl)
        };

        var client = new OpenAIClient(new ApiKeyCredential(apiKey), openAIOptions);

        var chatMessages = messages.Select<Models.ChatMessage, OpenAI.Chat.ChatMessage>(m => 
        {
            if (m.Role.ToLower() == "user")
                return OpenAI.Chat.ChatMessage.CreateUserMessage(m.Content);
            else if (m.Role.ToLower() == "assistant")
                return OpenAI.Chat.ChatMessage.CreateAssistantMessage(m.Content);
            else
                return OpenAI.Chat.ChatMessage.CreateSystemMessage(m.Content);
        }).ToList();

        var chatClient = client.GetChatClient(model);
        
        AsyncCollectionResult<StreamingChatCompletionUpdate> streamingResult = null;
        string errorMessage = null;
        
        try
        {
            streamingResult = chatClient.CompleteChatStreamingAsync(chatMessages, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            errorMessage = $"Error: {ex.Message}";
        }
        
        if (errorMessage != null)
        {
            yield return errorMessage;
            yield break;
        }

        await foreach (var update in streamingResult.ConfigureAwait(false))
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            foreach (var contentPart in update.ContentUpdate)
            {
                yield return contentPart.Text;
            }
        }
    }
}
