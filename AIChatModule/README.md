# AI Chat Module

An AI-powered chat interface module for OrchardCore CMS, featuring real-time streaming via Server-Sent Events (SSE) and OpenRouter integration.

## Features

- 🎨 **ChatGPT-style Interface** - Modern, dark-themed chat UI
- ⚡ **Real-time Streaming** - Server-Sent Events (SSE) for instant responses
- 🤖 **OpenRouter Integration** - Access multiple AI models through OpenRouter
- 📱 **Responsive Design** - Works seamlessly on desktop and mobile
- ✨ **Markdown Support** - Format code, bold text, and more in messages

## Requirements

- OrchardCore 2.2.1+
- .NET 8.0
- OpenRouter API key (from https://openrouter.ai/)

## Installation

1. The module is already included in the DashboardApplication

2. Enable the feature:
   - Navigate to Admin → Features
   - Find "AI Chat Module" under "Content Management"
   - Click "Enable"

3. Configure your OpenRouter API key (see Configuration below)

4. Access the chat at `/chat`

## Configuration

### Option 1: appsettings.json

Add your API key to `DashboardApplication/appsettings.json`:

```json
{
  "OpenRouter": {
    "ApiKey": "sk-or-v1-your-api-key-here"
  }
}
```

### Option 2: Environment Variable

Set the `OPENROUTER_API_KEY` environment variable:

```bash
# Linux/Mac
export OPENROUTER_API_KEY="sk-or-v1-your-api-key-here"

# Windows
set OPENROUTER_API_KEY=sk-or-v1-your-api-key-here
```

### Getting an OpenRouter API Key

1. Visit https://openrouter.ai/
2. Sign up for an account
3. Navigate to your API keys section
4. Create a new API key
5. Copy and configure it as described above

## Usage

1. Navigate to `/chat` in your browser
2. Type your message in the input field at the bottom
3. Press Enter or click the send button
4. Watch as the AI streams its response in real-time

## Technical Details

### Architecture

- **Backend**: 
  - OpenAI SDK for chat completions
  - Microsoft.Extensions.AI.Abstractions for standardized AI interfaces
  - Server-Sent Events for streaming responses
  
- **Frontend**:
  - Vanilla JavaScript (no framework dependencies)
  - EventSource API for SSE consumption
  - Custom CSS with dark theme

### File Structure

```
AIChatModule/
├── Controllers/
│   └── ChatController.cs          # Handles chat requests and SSE streaming
├── Models/
│   └── ChatModels.cs               # Data models for chat messages
├── Services/
│   └── OpenRouterService.cs        # OpenRouter integration service
├── Views/
│   └── Chat/
│       └── Index.cshtml            # Chat interface view
├── wwwroot/
│   ├── css/
│   │   └── chat.css                # Styling for chat interface
│   └── js/
│       └── chat.js                 # Client-side chat logic
├── AIChatModule.csproj             # Project file
├── Manifest.cs                     # Module manifest
└── Startup.cs                      # Service registration
```

### Supported AI Models

The module is configured to use `openai/gpt-3.5-turbo` by default, but can be easily modified to use any model supported by OpenRouter.

## Customization

### Changing the AI Model

Edit `AIChatModule/wwwroot/js/chat.js` and modify the model parameter:

```javascript
body: JSON.stringify({
    messages: this.messages,
    model: 'anthropic/claude-3-sonnet'  // Change this to your preferred model
})
```

### Styling

The chat interface uses a dark theme by default. To customize:

1. Edit `AIChatModule/wwwroot/css/chat.css`
2. Modify colors, fonts, spacing, etc.
3. Rebuild the application

## Troubleshooting

### Chat not loading

- Ensure the module is enabled in Admin → Features
- Check that the application has been restarted after configuration changes

### API errors

- Verify your OpenRouter API key is valid
- Check that you have sufficient credits in your OpenRouter account
- Review browser console for error messages

### SSE connection issues

- Ensure your server supports long-running connections
- Check firewall and proxy settings
- Verify `X-Accel-Buffering` is set to `no` for Nginx

## License

This module is part of the OrchardDashboard project.

## Support

For issues or questions:
- Open an issue on GitHub
- Check OrchardCore documentation at https://docs.orchardcore.net/
