# AI Chat Module

An OrchardCore module that provides an AI chat interface with Server-Sent Events (SSE) streaming support using OpenRouter as the AI service provider.

## Features

- **ChatGPT-like Interface**: Clean, modern chat UI inspired by ChatGPT
- **Real-time Streaming**: Uses Server-Sent Events (SSE) for real-time response streaming
- **OpenRouter Integration**: Connect to various AI models through OpenRouter API
- **Model Selection**: Choose from multiple AI models including GPT-3.5, GPT-4, Claude, and Gemini
- **Conversation History**: Maintains context throughout the conversation

## Prerequisites

- .NET 8.0 or later
- OrchardCore 2.2.1 or later
- OpenRouter API key ([Get one here](https://openrouter.ai/))

## Configuration

### 1. Get an OpenRouter API Key

1. Visit [OpenRouter.ai](https://openrouter.ai/)
2. Sign up for an account
3. Generate an API key from your dashboard

### 2. Configure the API Key

You can configure the OpenRouter API key in two ways:

#### Option A: Using appsettings.json

Edit `DashboardApplication/appsettings.json` or `appsettings.Development.json`:

```json
{
  "AIChatModule": {
    "ApiKey": "your-openrouter-api-key-here",
    "BaseUrl": "https://openrouter.ai/api/v1",
    "DefaultModel": "openai/gpt-3.5-turbo"
  }
}
```

#### Option B: Using Environment Variable

Set the `OPENROUTER_API_KEY` environment variable:

**Windows:**
```powershell
$env:OPENROUTER_API_KEY="your-openrouter-api-key-here"
```

**Linux/Mac:**
```bash
export OPENROUTER_API_KEY="your-openrouter-api-key-here"
```

## Available Models

The module supports various AI models through OpenRouter:

- `openai/gpt-3.5-turbo` - Fast and cost-effective
- `openai/gpt-4` - Most capable OpenAI model
- `anthropic/claude-3-haiku` - Fast Claude model
- `anthropic/claude-3-sonnet` - Balanced Claude model
- `google/gemini-pro` - Google's Gemini model

You can add more models by editing the dropdown in `Views/Chat/Index.cshtml`.

## Usage

### Accessing the Chat Interface

Once the module is enabled, navigate to:

```
http://localhost:5000/chat
```

Or your deployed application URL with the `/chat` path.

### Using the Chat

1. Type your message in the text input at the bottom
2. Press Enter or click the send button
3. Watch as the AI response streams in real-time
4. Select different models from the dropdown to try different AI providers

### Keyboard Shortcuts

- **Enter**: Send message
- **Shift + Enter**: New line in message

## Technical Architecture

### Backend Components

- **ChatController**: Handles HTTP requests and SSE streaming
  - `GET /chat`: Serves the chat interface
  - `POST /chat/stream`: SSE endpoint for streaming AI responses

- **OpenRouterChatService**: Manages communication with OpenRouter API
  - Converts messages to OpenAI format
  - Handles streaming responses
  - Error handling and cancellation support

- **Models**: Data structures for chat messages and settings
  - `ChatMessage`: Individual chat messages
  - `ChatRequest`: Request payload
  - `ChatSettings`: Configuration settings

### Frontend Components

- **chat.js**: JavaScript for handling:
  - SSE connection management
  - Message sending and receiving
  - UI updates and markdown rendering
  - Conversation history management

- **chat.css**: Modern, dark-themed UI styling inspired by ChatGPT

### SSE (Server-Sent Events) Flow

1. Client sends POST request with conversation history
2. Server establishes SSE connection
3. Server streams AI response tokens as they arrive
4. Client receives and displays tokens in real-time
5. Connection closes when response is complete

## Development

### Adding New Features

To extend the module:

1. **Add new models**: Edit the dropdown in `Views/Chat/Index.cshtml`
2. **Customize styling**: Modify `wwwroot/css/chat.css`
3. **Add features**: Extend `ChatController` and `chat.js`

### Project Structure

```
AIChatModule/
├── Controllers/
│   └── ChatController.cs      # HTTP endpoints
├── Models/
│   └── ChatModels.cs          # Data models
├── Services/
│   └── OpenRouterChatService.cs  # AI integration
├── Views/
│   └── Chat/
│       └── Index.cshtml       # Chat UI
├── wwwroot/
│   ├── css/
│   │   └── chat.css           # Styles
│   └── js/
│       └── chat.js            # Client logic
├── AIChatModule.csproj        # Project file
├── Manifest.cs                # Module metadata
└── Startup.cs                 # Service configuration
```

## Troubleshooting

### "Error: API key not configured"

Make sure you've configured the OpenRouter API key in either:
- `appsettings.json` under `AIChatModule:ApiKey`
- Environment variable `OPENROUTER_API_KEY`

### Chat not loading

Ensure the AIChatModule is enabled in OrchardCore:
1. Go to Admin → Features
2. Find "AI Chat Module"
3. Enable it if disabled

### SSE connection issues

- Check browser console for errors
- Verify firewall allows SSE connections
- Test with a simple message first

## Security Considerations

- **API Key Protection**: Never commit API keys to source control
- **Rate Limiting**: Consider implementing rate limiting for production
- **Input Validation**: The module validates and sanitizes user input
- **HTTPS**: Always use HTTPS in production for secure communication

## Cost Management

OpenRouter charges based on token usage:
- Monitor your usage in the OpenRouter dashboard
- Set spending limits to avoid unexpected charges
- Choose cost-effective models for testing (e.g., GPT-3.5 Turbo)

## License

This module is part of the OrchardDashboard project and follows the same license.

## Support

For issues and questions:
- Check the [OrchardCore documentation](https://docs.orchardcore.net/)
- Visit [OpenRouter documentation](https://openrouter.ai/docs)
- Open an issue on the GitHub repository
