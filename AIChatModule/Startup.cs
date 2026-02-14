using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.AI;
using AIChatModule.Models;
using AIChatModule.Services;
using OpenAI;
using System;
using System.ClientModel;

namespace AIChatModule
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Bind OpenRouter settings from configuration
            var openRouterSettings = new OpenRouterSettings();
            _configuration.GetSection("OpenRouter").Bind(openRouterSettings);

            // Register IChatClient using Microsoft.Extensions.AI abstraction
            // This uses the OpenAI SDK but wraps it in the IChatClient interface
            services.AddSingleton<IChatClient>(sp =>
            {
                var credential = new ApiKeyCredential(openRouterSettings.ApiKey);
                var openAiClient = new OpenAIClient(
                    credential,
                    new OpenAIClientOptions 
                    { 
                        Endpoint = new Uri(openRouterSettings.BaseUrl)
                    });

                // Use AsIChatClient() extension method to get IChatClient abstraction
                return openAiClient.GetChatClient(openRouterSettings.Model).AsIChatClient();
            });

            // Register chat service
            services.AddScoped<IChatService, OpenRouterChatService>();

            // Add MVC controllers
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes)
        {
            // Routes are handled by controller attributes
        }
    }
}
