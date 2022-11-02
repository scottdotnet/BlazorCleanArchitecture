using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Domain.Tenant;
using BlazorCleanArchitecture.Infrastructure.Common;
using BlazorCleanArchitecture.Infrastructure.Data;
using BlazorCleanArchitecture.Shared.User.User;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BlazorCleanArchitecture.Application.IntegrationTests
{
    public sealed class TestFixture : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public readonly TestcontainerDatabase Database = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration
            {
                Password = "Abcdefgh1!"
            })
            .Build();

        public UserDto CurrentUser { get; set; } = new UserDto { Id = 1, Username = "test@test.com" };
        public Tenant Tenant { get; set; } = new Tenant { Id = 1, Name = "Test", Domain = "test.com" };

        private HttpContext HttpContext
        {
            get
            {
                var httpContext = new DefaultHttpContext { };
                httpContext.Request.Headers.Add("X-Tenant-Domain", CurrentUser.Username.Split("@")[1]);

                return httpContext;
            }
        }

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(2);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IHttpContextAccessor>()
                    .AddTransient(provider => Mock.Of<IHttpContextAccessor>(s => s.HttpContext == HttpContext));

                services.RemoveAll<ITenantService>()
                    .AddTransient(provider => Mock.Of<ITenantService>(s => s.Tenant == Tenant));

                services.RemoveAll<DbContextOptions<ApplicationDbContext>>()
                    .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Database.ConnectionString, x=> x.MigrationsAssembly(InfrastructureAssembly.Assembly.FullName)));
            });
        }

        public Task InitializeAsync() => Setup();

        public new Task DisposeAsync() => Cleanup();

        public async Task Setup()
        {
            await Semaphore.WaitAsync();

            await Database.StartAsync();
        }

        public async Task Cleanup()
        {
            await Database.StopAsync();

            Semaphore.Release();
        }
    }
}
