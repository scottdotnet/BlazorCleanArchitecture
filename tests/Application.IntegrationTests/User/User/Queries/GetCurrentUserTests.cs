using BlazorCleanArchitecture.Infrastructure.Data;
using BlazorCleanArchitecture.Shared.User.User.Queries;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit;
using BlazorCleanArchitecture.Shared.Common.Exceptions;

namespace BlazorCleanArchitecture.Application.IntegrationTests.User.User.Queries
{
    public sealed class GetCurrentUserTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _testing;
        private readonly ITestOutputHelper _testOutputHelper;

        public GetCurrentUserTests(TestFixture testing, ITestOutputHelper testOutputHelper)
        {
            _testing = testing;
            _testOutputHelper = testOutputHelper;
        }

        internal async Task Setup()
        {
            var tenant = new Domain.Tenant.Tenant
            {
                Name = "Test",
                Domain = _testing.CurrentUser.Username.Split("@")[1]
            };

            await _testing.AddAsync<ApplicationDbContext, Domain.Tenant.Tenant>(tenant);

            await _testing.AddAsync<ApplicationDbContext, Domain.User.User>(new Domain.User.User
            {
                FirstName = "Test",
                LastName = "Tester",
                Username = "test@test.com",
                Email = "test@test.com",
                Password = "Abcdefgh1!",
                TenantId = tenant.Id
            });
        }

        internal async Task Cleanup()
        {
            await _testing.ClearAsync<ApplicationDbContext, Domain.User.User>();
        }

        public Task DisposeAsync()
        {
            return Cleanup();
        }

        public Task InitializeAsync()
        {
            return Setup();
        }

        [Fact]
        public async Task CurrentUserExists()
        {
            _testing.CurrentUser.Username = "test@test.com";

            var result = await _testing.SendAsync(new GetCurrentUser { });

            result.Username.Should().Be("test@test.com");
        }

        [Fact]
        public async Task CurrentUserDoesntExist()
        {
            _testing.CurrentUser.Username = "abc@test.com";

            var act = async () => await _testing.SendAsync(new GetCurrentUser { });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "User", new[] { "User does not exist." } }
            });
        }
    }
}
