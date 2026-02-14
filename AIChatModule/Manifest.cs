using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "AI Chat",
    Author = "OrchardDashboard Team",
    Website = "https://github.com/shinexyt/OrchardDashboard",
    Version = "1.0.0"
)]

[assembly: Feature(
    Id = "AIChatModule",
    Name = "AI Chat Module",
    Description = "AI-powered chat interface using OpenRouter and Server-Sent Events (SSE).",
    Category = "Content Management"
)]
