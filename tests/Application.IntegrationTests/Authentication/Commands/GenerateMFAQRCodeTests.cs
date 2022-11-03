using BlazorCleanArchitecture.Domain.Tenant;
using BlazorCleanArchitecture.Domain.User;
using BlazorCleanArchitecture.Infrastructure.Data;
using BlazorCleanArchitecture.Shared.Authentication.Commands;
using BlazorCleanArchitecture.Shared.Common.Exceptions;
using FluentAssertions;
using FluentAssertions.Equivalency;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BlazorCleanArchitecture.Application.IntegrationTests.Authentication.Commands
{
    public sealed class GenerateMFAQRCodeTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _testing;

        private Domain.User.User User = new Domain.User.User
        {
            FirstName = "Test",
            LastName = "Tester",
            Username = "test@test.com",
            Email = "test@test.com",
            Password = "Abcdefgh1!",
            MFAKey = Guid.NewGuid()
        };

        public GenerateMFAQRCodeTests(TestFixture testing)
            => _testing = testing;

        internal async Task Setup()
        {
            var tenant = new Tenant
            {
                Name = "Test",
                Domain = _testing.CurrentUser.Username.Split("@")[1]
            };

            await _testing.AddAsync<ApplicationDbContext, Tenant>(tenant);

            User.TenantId = tenant.Id;

            await _testing.AddAsync<ApplicationDbContext, Domain.User.User>(User);
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
        public async Task SuccessfullyGenerate()
        {
            var result = await _testing.SendAsync(new GenerateMFAQRCode { UserId = User.Id });

            result.Should().NotBeNullOrWhiteSpace();
            result.Should().StartWith("data:image/png;base64");
        }

        [Fact]
        public async Task UserIdEmpty()
        {
            var act = async () => await _testing.SendAsync(new GenerateMFAQRCode { });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "UserId", new[] { "'User Id' must not be empty." } }
            });
        }

        [Fact]
        public async Task InvalidUser()
        {
            var act = async () => await _testing.SendAsync(new GenerateMFAQRCode { UserId = 100 });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "User", new[] { "User does not exist." } }
            });
        }

        [Fact]
        public async Task UserHasNoKey()
        {
            User.MFAKey = Guid.Empty;

            await _testing.UpdateAsync<ApplicationDbContext, Domain.User.User>(User);

            var act = async () => await _testing.SendAsync(new GenerateMFAQRCode { UserId = User.Id });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "User", new[] { "User does not have an MFA Key." } }
            });
        }
    }
}
