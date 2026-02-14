using AIChatModule.Models;
using AIChatModule.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIChatModule.Controllers;

public class ChatController : Controller
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet("/chat")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("/chat/stream")]
    public async Task StreamChat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        try
        {
            await foreach (var token in _chatService.StreamChatAsync(
                request.Messages, 
                request.Model ?? "openai/gpt-3.5-turbo",
                cancellationToken))
            {
                var data = $"data: {token}\n\n";
                await Response.WriteAsync(data, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }

            await Response.WriteAsync("data: [DONE]\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            var errorData = $"data: Error: {ex.Message}\n\n";
            await Response.WriteAsync(errorData, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}
