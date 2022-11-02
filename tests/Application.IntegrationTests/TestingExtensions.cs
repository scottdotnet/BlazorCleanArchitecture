using Mediator;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Application.IntegrationTests
{
    public static class TestingExtensions
    {
        private static IServiceScope GetScope(this WebApplicationFactory<Program> factory)
            => factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        private static T GetRequiredService<T>(this IServiceScope scope) where T : notnull
            => scope.ServiceProvider.GetRequiredService<T>();

        public static async Task CreateAndMigrateAsync<TContext>(this WebApplicationFactory<Program> factory)
            where TContext : DbContext
        {
            using var scope = factory.GetScope();

            var context = scope.GetRequiredService<TContext>();

            await context.Database.MigrateAsync();
        }

        public static async Task<TResponse> SendAsync<TResponse>(this WebApplicationFactory<Program> factory, IRequest<TResponse> request)
            => await factory.GetScope().GetRequiredService<Mediator.Mediator>().Send(request);

        public static async Task PublishAsync(this WebApplicationFactory<Program> factory, INotification request)
            => await factory.GetScope().GetRequiredService<Mediator.Mediator>().Publish(request);

        public static DbSet<TEntity> Entity<TContext, TEntity>(this WebApplicationFactory<Program> factory)
            where TContext : DbContext
            where TEntity : class
            => factory.GetScope().GetRequiredService<TContext>().Set<TEntity>();

        public static async Task<TEntity?> FindAsync<TContext, TEntity>(this WebApplicationFactory<Program> factory, params object[] keyValues)
            where TContext : DbContext
            where TEntity : class
            => await factory.GetScope().GetRequiredService<TContext>().FindAsync<TEntity>(keyValues);

        public static async Task AddAsync<TContext, TEntity>(this WebApplicationFactory<Program> factory, params TEntity[] entities)
            where TContext : DbContext
            where TEntity : class
        {
            using var scope = factory.GetScope();

            var context = scope.GetRequiredService<TContext>();

            await context.AddRangeAsync(entities);

            await context.SaveChangesAsync();
        }

        public static async Task ClearAsync<TContext, TEntity>(this WebApplicationFactory<Program> factory)
            where TContext : DbContext
            where TEntity : class
        {
            using var scope = factory.GetScope();

            var context = scope.GetRequiredService<TContext>();

            context.RemoveRange(context.Set<TEntity>().ToList());

            await context.SaveChangesAsync();
        }

        public static async Task<int> CountAsync<TContext, TEntity>(this WebApplicationFactory<Program> factory)
            where TContext : DbContext
            where TEntity : class
            => await factory.GetScope().GetRequiredService<TContext>().Set<TEntity>().CountAsync();

        public static async Task<List<TEntity>> ListAsync<TContext, TEntity>(this WebApplicationFactory<Program> factory)
            where TContext : DbContext
            where TEntity : class
            => await factory.GetScope().GetRequiredService<TContext>().Set<TEntity>().ToListAsync();
    }
}
