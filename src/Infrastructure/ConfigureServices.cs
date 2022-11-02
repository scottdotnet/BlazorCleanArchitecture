using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Infrastructure.Common;
using BlazorCleanArchitecture.Infrastructure.Common.EntityFramework;
using BlazorCleanArchitecture.Infrastructure.Data;
using BlazorCleanArchitecture.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BlazorCleanArchitecture.Infrastructure
{
    public static class ConfigureServices
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabase(configuration);
            services.AddServices();
        }

        private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("Default"), x => x.MigrationsAssembly(InfrastructureAssembly.Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IMemoryCache, MemoryCache>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ITenantService, TenantService>();

            InfrastructureAssembly
                .Assembly
                .GetTypes()
                .Where(t => t.IsAbstract is false)
                .Where(t => t.IsGenericTypeDefinition is false)
                .Where(t => typeof(EntityTypeConfigurationDependency).IsAssignableFrom(t))
                .ToList()
                .ForEach(t => services.AddScoped(typeof(EntityTypeConfigurationDependency), t));
        }
    }
}
