using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenAI.Chat;
using AppChatMessage = AIChatModule.Models.ChatMessage;
using OpenAIChatMessage = OpenAI.Chat.ChatMessage;

namespace AIChatModule.Services
{
    public interface IChatService
    {
        IAsyncEnumerable<string> GetStreamingResponseAsync(List<AppChatMessage> messages, CancellationToken cancellationToken = default);
    }

    public class OpenRouterChatService : IChatService
    {
        private readonly ChatClient _chatClient;

        public OpenRouterChatService(ChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async IAsyncEnumerable<string> GetStreamingResponseAsync(
            List<AppChatMessage> messages, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var chatMessages = messages.Select(m => 
                m.Role switch
                {
                    "user" => new UserChatMessage(m.Content) as OpenAIChatMessage,
                    "assistant" => new AssistantChatMessage(m.Content) as OpenAIChatMessage,
                    "system" => new SystemChatMessage(m.Content) as OpenAIChatMessage,
                    _ => new UserChatMessage(m.Content) as OpenAIChatMessage
                }
            ).ToList();

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
