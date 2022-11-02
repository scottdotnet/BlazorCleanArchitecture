using AutoMapper.EquivalencyExpression;
using BlazorCleanArchitecture.Application.Common;
using BlazorCleanArchitecture.Application.Common.Behaviours;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BlazorCleanArchitecture.Application
{
    public static class ConfigureServices
    {
        public static void AddApplication(this IServiceCollection services, params Assembly[] assemblies)
        {
            assemblies = assemblies.Concat(new[] { ApplicationAssembly.Assembly }).ToArray();

            services.AddAutoMapper(options => options.AddCollectionMappers(), assemblies);
            services.AddValidatorsFromAssemblies(assemblies);
            services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);

            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
            ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        }
    }
}
