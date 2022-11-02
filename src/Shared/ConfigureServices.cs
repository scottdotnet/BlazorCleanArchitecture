using BlazorCleanArchitecture.Shared.Common.Interfaces;
using BlazorCleanArchitecture.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorCleanArchitecture.Shared
{
    public static class ConfigureServices
    {
        public static void AddSharedServices(this IServiceCollection services)
        {
            services.AddTransient<IDateTimeService, DateTimeService>();
        }
    }
}
