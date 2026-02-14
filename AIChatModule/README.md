# AI Chat Module

An OrchardCore module that provides AI-powered chat functionality using Server-Sent Events (SSE) for real-time streaming responses.

## Features

- **Real-time Streaming**: Uses Server-Sent Events (SSE) for streaming AI responses
- **ChatGPT-Style UI**: Modern, responsive chat interface similar to ChatGPT
- **OpenRouter Integration**: Connects to OpenRouter API for AI model access
- **OrchardCore Integration**: Seamlessly integrates with OrchardCore modular application framework

## Prerequisites

- .NET 8.0 SDK or later
- OrchardCore 2.2.1 or later
- OpenRouter API key

## Installation

1. The module is already added to the OrchardDashboard solution
2. Configure your OpenRouter API key. **Important**: Never commit API keys to source control!

**Option 1: User Secrets (Recommended for Development)**
```bash
cd DashboardApplication
dotnet user-secrets set "OpenRouter:ApiKey" "YOUR_API_KEY_HERE"
```

**Option 2: Environment Variables (Recommended for Production)**
```bash
export OpenRouter__ApiKey="YOUR_API_KEY_HERE"
```

**Option 3: Configuration File (Not Recommended)**
Update `appsettings.json` or `appsettings.Development.json`:
```json
{
  "OpenRouter": {
    "ApiKey": "YOUR_OPENROUTER_API_KEY_HERE",
    "BaseUrl": "https://openrouter.ai/api/v1",
    "Model": "openai/gpt-3.5-turbo"
  }
}
```

3. Run the DashboardApplication project
4. Complete the setup wizard if it's the first time running
5. Enable the "AI Chat Module" feature from the Features admin page (if not already enabled)

## Usage

Once the module is enabled:

1. Navigate to `/chat` in your browser
2. Type your message in the input field at the bottom
3. Press Enter or click the send button
4. Watch as the AI response streams in real-time

## Architecture

### Backend

- **ChatController**: Handles HTTP requests for the chat interface and streaming endpoint
  - `GET /chat`: Serves the chat UI
  - `POST /chat/stream`: SSE endpoint for streaming AI responses

- **ChatService**: Implements the AI chat logic using Microsoft.Extensions.AI abstractions
  - Uses `IChatClient` interface from Microsoft.Extensions.AI
  - Connects to OpenRouter via OpenAI SDK wrapped in IChatClient abstraction
  - Streams responses using `GetStreamingResponseAsync` method

- **Startup**: Configures dependency injection
  - Registers `IChatClient` using `AsIChatClient()` extension method
  - Wraps OpenAI SDK's ChatClient in Microsoft.Extensions.AI abstraction layer
  - Registers `ChatService` for chat operations
  - Adds MVC controllers for routing

### Frontend

- **Index.cshtml**: Main chat interface view
- **chat.css**: ChatGPT-inspired styling with dark theme
- **chat.js**: JavaScript client for:
  - Managing chat state
  - Sending messages to backend
  - Receiving and displaying streaming responses via SSE
  - Auto-scrolling and message formatting

## Technical Details

- **SSE Implementation**: The backend sends data in the `data: {json}\n\n` format
- **Microsoft.Extensions.AI**: Uses the official .NET AI abstractions (v10.3.0)
  - `IChatClient` interface provides provider-agnostic chat functionality
  - `GetStreamingResponseAsync` method for streaming responses
  - `AsIChatClient()` extension converts OpenAI SDK client to IChatClient
- **OpenAI SDK**: Leverages OpenAI SDK for OpenRouter compatibility
- **Streaming**: Real-time token-by-token response streaming for better UX

## Configuration Options

### OpenRouter Settings

- `ApiKey`: Your OpenRouter API key
- `BaseUrl`: OpenRouter API endpoint (default: `https://openrouter.ai/api/v1`)
- `Model`: AI model to use (default: `openai/gpt-3.5-turbo`)

Available models include:
- `openai/gpt-3.5-turbo`
- `openai/gpt-4`
- `anthropic/claude-2`
- Many more available on OpenRouter

## Troubleshooting

### Module Not Loading

If the `/chat` endpoint returns 404:
1. Check that the module is enabled in the Features admin page
2. Verify the module is added to the setup recipe
3. Check application logs for any startup errors

### API Key Errors

If you see authentication errors:
1. Verify your OpenRouter API key is correct
2. Check that the API key is properly set in appsettings
3. Ensure you have credits available on your OpenRouter account

### Streaming Not Working

If responses don't stream:
1. Check browser console for JavaScript errors
2. Verify SSE connection is established
3. Check network tab for the `/chat/stream` request

## Development

The module follows OrchardCore best practices:
- Uses `OrchardCore.Module.Targets` for module packaging
- Implements proper dependency injection
- Follows ASP.NET Core conventions
- Uses standard OrchardCore manifest

## Future Enhancements

Potential improvements:
- Add conversation history persistence
- Support multiple AI models/providers
- Add user authentication and per-user chat history
- Implement rate limiting
- Add admin UI for configuration
- Support for file uploads and images
- Markdown rendering for responses

## License

Same as OrchardDashboard project license.
