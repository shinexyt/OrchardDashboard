using AIChatModule.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using System;

namespace AIChatModule
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            // Register OpenRouter service
            services.AddSingleton<IOpenRouterService, OpenRouterService>();
            
            // Add MVC controllers
            services.AddControllersWithViews();
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            // Map controller routes
            routes.MapControllerRoute(
                name: "AIChatModule",
                pattern: "chat/{action=Index}/{id?}",
                defaults: new { controller = "Chat" }
            );
        }
    }
}
