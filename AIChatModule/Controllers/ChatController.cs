using AIChatModule.Models;
using AIChatModule.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AIChatModule.Controllers
{
    [Route("chat")]
    public class ChatController : Controller
    {
        private readonly IOpenRouterService _openRouterService;

        public ChatController(IOpenRouterService openRouterService)
        {
            _openRouterService = openRouterService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("api/stream")]
        public async Task StreamChat([FromBody] ChatRequest request, CancellationToken cancellationToken)
        {
            // Set up SSE response headers
            Response.Headers["Content-Type"] = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";
            Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                await foreach (var chunk in _openRouterService.StreamChatCompletionAsync(
                    request.Messages,
                    request.Model,
                    cancellationToken))
                {
                    var data = JsonSerializer.Serialize(new ChatStreamChunk
                    {
                        Content = chunk,
                        Done = false
                    });

                    await Response.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes($"data: {data}\n\n"), cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);
                }

                // Send done message
                var doneData = JsonSerializer.Serialize(new ChatStreamChunk
                {
                    Content = "",
                    Done = true
                });
                await Response.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes($"data: {doneData}\n\n"), cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
            catch (System.Exception ex)
            {
                var errorData = JsonSerializer.Serialize(new
                {
                    error = ex.Message,
                    done = true
                });
                await Response.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes($"data: {errorData}\n\n"), cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
    }
}
