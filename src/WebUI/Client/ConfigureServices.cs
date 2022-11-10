using BlazorCleanArchitecture.Shared.Common;
using BlazorCleanArchitecture.Shared.Common.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Refit;

namespace BlazorCleanArchitecture.WebUI.Client
{
    public static class ConfigureServices
    {
        public static void AddWebClientServices(this IServiceCollection services, IConfiguration configuration, IWebAssemblyHostEnvironment environment)
        {
            services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(environment.BaseAddress) });

            services.AddOptions();
            services.AddAuthorizationCore();

            services.AddRefitClients(environment);

            services.AddMudServices();
        }

        private static void AddRefitClients(this IServiceCollection services, IWebAssemblyHostEnvironment environment)
        {
            var endpoints = SharedAssembly
                .Assembly
                .GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i == typeof(IEndpoint)))
                .Where(t => t.IsInterface);

            foreach (var endpoint in endpoints)
                services.AddRefitClient(endpoint)
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{environment.BaseAddress}api"));
        }
    }
}
