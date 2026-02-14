using AIChatModule.Models;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using OpenAIChatMessage = OpenAI.Chat.ChatMessage;

namespace AIChatModule.Services
{
    public interface IOpenRouterService
    {
        IAsyncEnumerable<string> StreamChatCompletionAsync(
            List<Models.ChatMessage> messages, 
            string model,
            CancellationToken cancellationToken = default);
    }

    public class OpenRouterService : IOpenRouterService
    {
        private readonly ChatClient _chatClient;
        private readonly IConfiguration _configuration;

        public OpenRouterService(IConfiguration configuration)
        {
            _configuration = configuration;
            
            // Get API key from configuration
            var apiKey = _configuration["OpenRouter:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ?? "sk-or-v1-dummy-key";
            
            // Create OpenAI client configured to use OpenRouter
            var options = new OpenAI.OpenAIClientOptions
            {
                Endpoint = new Uri("https://openrouter.ai/api/v1")
            };
            
            var openAIClient = new OpenAI.OpenAIClient(new ApiKeyCredential(apiKey), options);
            _chatClient = openAIClient.GetChatClient("openai/gpt-3.5-turbo");
        }

        public async IAsyncEnumerable<string> StreamChatCompletionAsync(
            List<Models.ChatMessage> messages,
            string model,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Convert messages to OpenAI format
            var chatMessages = messages.Select<Models.ChatMessage, OpenAIChatMessage>(m => 
                m.Role.ToLower() switch
                {
                    "system" => OpenAIChatMessage.CreateSystemMessage(m.Content),
                    "assistant" => OpenAIChatMessage.CreateAssistantMessage(m.Content),
                    _ => OpenAIChatMessage.CreateUserMessage(m.Content)
                }
            ).ToList();

            // Stream the response
            await foreach (var update in _chatClient.CompleteChatStreamingAsync(chatMessages, cancellationToken: cancellationToken))
            {
                foreach (var contentPart in update.ContentUpdate)
                {
                    if (!string.IsNullOrEmpty(contentPart.Text))
                    {
                        yield return contentPart.Text;
                    }
                }
            }
        }
    }
}
