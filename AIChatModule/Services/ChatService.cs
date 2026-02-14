using Microsoft.Extensions.AI;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppChatMessage = AIChatModule.Models.ChatMessage;

namespace AIChatModule.Services
{
    public interface IChatService
    {
        IAsyncEnumerable<string> GetStreamingResponseAsync(List<AppChatMessage> messages, CancellationToken cancellationToken = default);
    }

    public class OpenRouterChatService : IChatService
    {
        private readonly IChatClient _chatClient;

        public OpenRouterChatService(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async IAsyncEnumerable<string> GetStreamingResponseAsync(
            List<AppChatMessage> messages, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Convert app messages to Microsoft.Extensions.AI ChatMessage format
            var chatMessages = messages.Select(m => 
                new ChatMessage(
                    m.Role switch
                    {
                        "user" => ChatRole.User,
                        "assistant" => ChatRole.Assistant,
                        "system" => ChatRole.System,
                        _ => ChatRole.User
                    },
                    m.Content
                )
            ).ToList();

            // Use GetStreamingResponseAsync from IChatClient (the correct method name)
            await foreach (var update in _chatClient.GetStreamingResponseAsync(chatMessages, cancellationToken: cancellationToken))
            {
                // Extract text from the streaming response
                if (!string.IsNullOrEmpty(update.Text))
                {
                    yield return update.Text;
                }
            }
        }
    }
}
