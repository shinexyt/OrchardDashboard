using Microsoft.AspNetCore.Mvc;
using AIChatModule.Models;
using AIChatModule.Services;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace AIChatModule.Controllers
{
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
        [IgnoreAntiforgeryToken]
        public async Task StreamChat([FromBody] ChatRequest request, CancellationToken cancellationToken)
        {
            Response.Headers["Content-Type"] = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";

            try
            {
                await foreach (var chunk in _chatService.GetStreamingResponseAsync(request.Messages, cancellationToken))
                {
                    var data = $"data: {JsonSerializer.Serialize(new { content = chunk })}\n\n";
                    await Response.Body.WriteAsync(Encoding.UTF8.GetBytes(data), cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);
                }

                // Send completion signal
                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes("data: [DONE]\n\n"), cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Client disconnected
            }
            catch (Exception ex)
            {
                var errorData = $"data: {JsonSerializer.Serialize(new { error = ex.Message })}\n\n";
                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes(errorData), cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
    }
}
