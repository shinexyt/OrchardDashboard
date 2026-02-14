using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AIChatModule.Models;
using AIChatModule.Services;
using OpenAI;
using OpenAI.Chat;
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

            // Register ChatClient using OpenAI SDK with OpenRouter endpoint
            services.AddSingleton<ChatClient>(sp =>
            {
                var credential = new ApiKeyCredential(openRouterSettings.ApiKey);
                var openAiClient = new OpenAIClient(
                    credential,
                    new OpenAIClientOptions 
                    { 
                        Endpoint = new Uri(openRouterSettings.BaseUrl)
                    });

                return openAiClient.GetChatClient(openRouterSettings.Model);
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
