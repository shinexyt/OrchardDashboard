using AIChatModule.Models;
using AIChatModule.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace AIChatModule;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Configure chat settings
        services.Configure<ChatSettings>(options =>
        {
            options.ApiKey = _configuration["AIChatModule:ApiKey"] ?? 
                           Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ?? 
                           string.Empty;
            options.BaseUrl = _configuration["AIChatModule:BaseUrl"] ?? "https://openrouter.ai/api/v1";
            options.DefaultModel = _configuration["AIChatModule:DefaultModel"] ?? "openai/gpt-3.5-turbo";
        });

        // Register chat service
        services.AddScoped<IChatService, OpenRouterChatService>();
    }

    public void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes)
    {
        // Serve static files from wwwroot
        var fileProvider = new ManifestEmbeddedFileProvider(
            typeof(Startup).Assembly,
            "wwwroot"
        );

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = fileProvider,
            RequestPath = "/AIChatModule"
        });
    }
}
