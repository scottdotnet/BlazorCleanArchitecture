using BlazorCleanArchitecture.Domain.User;
using BlazorCleanArchitecture.Infrastructure.Data;
using BlazorCleanArchitecture.Shared.Authentication.Commands;
using BlazorCleanArchitecture.Shared.Common.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BlazorCleanArchitecture.Application.IntegrationTests.Authentication.Commands
{
    public sealed class ForgotPasswordTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _testing;

        public ForgotPasswordTests(TestFixture testing)
            => _testing = testing;

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
            await _testing.ClearAsync<ApplicationDbContext, PasswordReset>();
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
        public async Task ForgotPasswordSuccessfully()
        {
            await _testing.SendAsync(new ForgotPassword { Username = "test@test.com" });

            var result = await _testing.Entity<ApplicationDbContext, PasswordReset>().Include(x => x.User).FirstAsync();

            result.Should().NotBeNull();
            result.User.Username.Should().Be("test@test.com");
        }

        [Fact]
        public async Task UserDoesntExist()
        {
            await _testing.SendAsync(new ForgotPassword { Username = "abc@def.xyz" });

            var result = await _testing.Entity<ApplicationDbContext, PasswordReset>().Include(x => x.User).FirstOrDefaultAsync();

            result.Should().BeNull();
        }

        [Fact]
        public async Task ResetAlreadyExists()
        {
            await _testing.SendAsync(new ForgotPassword { Username = "test@test.com" });

            var result = await _testing.Entity<ApplicationDbContext, PasswordReset>().Include(x => x.User).ToListAsync();

            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(1);
            result.First().Should().NotBeNull();
            result.First().User.Username.Should().Be("test@test.com");

            await _testing.SendAsync(new ForgotPassword { Username = "test@test.com" });

            result = await _testing.Entity<ApplicationDbContext, PasswordReset>().Include(x => x.User).ToListAsync();

            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(null, "'Username' must not be empty.")]
        [InlineData("", "'Username' must not be empty.")]
        [InlineData("abc", "'Username' is not a valid email address.")]
        [InlineData("testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest@test.com", "The length must be no more than 320 characters.")]
        public async Task InvalidUsername(string username, string expectedError)
        {
            var act = async () => await _testing.SendAsync(new ForgotPassword { Username = username });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "Username", new[] { expectedError } }
            });

            var count = await _testing.CountAsync<ApplicationDbContext, PasswordReset>();

            count.Should().Be(0);
        }
    }
}
